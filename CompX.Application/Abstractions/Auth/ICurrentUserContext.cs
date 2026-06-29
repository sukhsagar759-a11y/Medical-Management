namespace CompX.Application.Abstractions.Auth;

public interface ICurrentUserContext
{
    string UserName { get; }
    bool IsInRole(string roleName);
}
