using CompX.Application.Abstractions.Services;
using CompX.SharedKernel.Contracts.Common.Responses;

namespace CompX.Application.Services;

public sealed class SampleApplicationService : ISampleApplicationService
{
    public Task<SecureMessageResponse> GetSecureMessageAsync(string subject, CancellationToken cancellationToken = default)
        => Task.FromResult(new SecureMessageResponse($"Secure hello to {subject}."));
}
