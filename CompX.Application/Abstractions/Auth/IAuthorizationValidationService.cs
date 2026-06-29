namespace CompX.Application.Abstractions.Auth;

public interface IAuthorizationValidationService
{
    Task<bool> IsSessionValidAsync(string userId, CancellationToken cancellationToken = default);

    Task<bool> HasAnyPermissionAsync(string userId, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken = default);

    Task<bool> HasAllPermissionsAsync(string userId, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken = default);
}
