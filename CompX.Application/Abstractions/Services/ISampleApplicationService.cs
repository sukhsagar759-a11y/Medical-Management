using CompX.SharedKernel.Contracts.Common.Responses;

namespace CompX.Application.Abstractions.Services;

public interface ISampleApplicationService
{
    Task<SecureMessageResponse> GetSecureMessageAsync(string subject, CancellationToken cancellationToken = default);
}
