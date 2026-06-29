using CompX.SharedKernel.Contracts.Common.Responses;

namespace CompX.Application.Abstractions.Services;

public interface IHealthApplicationService
{
    Task<HealthResponse> GetAsync(CancellationToken cancellationToken = default);
}
