using CompX.Application.Abstractions.Services;
using CompX.SharedKernel.Contracts.Common.Responses;

namespace CompX.Application.Services;

public sealed class HealthApplicationService : IHealthApplicationService
{
    public Task<HealthResponse> GetAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new HealthResponse("CompX.Api", "Healthy", DateTimeOffset.UtcNow));
}
