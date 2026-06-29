using CompX.Application.Abstractions.Auth;
using CompX.SharedKernel.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CompX.Api.Security;

public sealed class RbacAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly IAuthorizationValidationService authorizationValidationService;
    private readonly ILogger<RbacAuthorizationFilter> logger;

    public RbacAuthorizationFilter(IAuthorizationValidationService authorizationValidationService, ILogger<RbacAuthorizationFilter> logger)
    {
        this.authorizationValidationService = authorizationValidationService;
        this.logger = logger;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (IsAnonymous(context))
        {
            return;
        }

        var userId = context.HttpContext.User.FindFirst("userid")?.Value
                     ?? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!await authorizationValidationService.IsSessionValidAsync(userId, context.HttpContext.RequestAborted))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Session expired or invalid." });
            return;
        }

        var permissions = GetRequiredPermissions(context);
        if (permissions.Count == 0)
        {
            return;
        }

        if (permissions.Any(permission => string.Equals(permission, PermissionCodes.AnyPermission, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var allowAll = permissions.Any(permission => string.Equals(permission, PermissionCodes.AllPermission, StringComparison.OrdinalIgnoreCase));
        var actualPermissions = permissions
            .Where(permission => !string.Equals(permission, PermissionCodes.AllPermission, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var hasAccess = allowAll
            ? await authorizationValidationService.HasAllPermissionsAsync(userId, actualPermissions, context.HttpContext.RequestAborted)
            : await authorizationValidationService.HasAnyPermissionAsync(userId, actualPermissions, context.HttpContext.RequestAborted);

        if (!hasAccess)
        {
            logger.LogWarning("Authorization denied for user {UserId} on {Controller}/{Action}.", userId, context.RouteData.Values["controller"], context.RouteData.Values["action"]);
            context.Result = new ForbidResult();
        }
    }

    private static bool IsAnonymous(AuthorizationFilterContext context)
    {
        return context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
    }

    private static IReadOnlyCollection<string> GetRequiredPermissions(AuthorizationFilterContext context)
    {
        var explicitPermissions = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowPermissionsAttribute>()
            .SelectMany(attribute => attribute.Permissions)
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Select(permission => permission.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (explicitPermissions.Length > 0)
        {
            return explicitPermissions;
        }

        var controller = context.RouteData.Values["controller"]?.ToString();
        var action = context.RouteData.Values["action"]?.ToString();
        if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action))
        {
            return Array.Empty<string>();
        }

        return [$"{controller}-{action}"];
    }
}
