using CompX.SharedKernel.Contracts.Auth;

namespace CompX.Api.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class AllowPermissionsAttribute : Attribute
{
    public AllowPermissionsAttribute(params string[] permissions)
    {
        Permissions = permissions?.Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Select(permission => permission.Trim())
            .ToArray() ?? Array.Empty<string>();
    }

    public IReadOnlyList<string> Permissions { get; }
}
