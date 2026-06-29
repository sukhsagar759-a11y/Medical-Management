using CompX.SharedKernel.Contracts.Auth.Requests;
using CompX.SharedKernel.Contracts.Auth.Responses;

namespace CompX.Application.Abstractions.Auth;

public interface IForgotPasswordService
{
    Task<ForgotPasswordResponse> CreateRequestAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<ForgotPasswordResponse> ValidateTokenAsync(ValidateForgotPasswordTokenRequest request, CancellationToken cancellationToken = default);
    Task<ForgotPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<ForgotPasswordResponse> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
}
