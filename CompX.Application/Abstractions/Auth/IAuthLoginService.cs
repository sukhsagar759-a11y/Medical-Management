using CompX.SharedKernel.Contracts.Auth.Requests;
using CompX.SharedKernel.Contracts.Auth.Responses;

namespace CompX.Application.Abstractions.Auth;

public interface IAuthLoginService
{
    Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default);
    Task<AuthenticateResponse> ValidateAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(string? userId, CancellationToken cancellationToken = default);
}
