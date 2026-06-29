using CompX.SharedKernel.Contracts.Scheduling.Dtos;

namespace CompX.Application.Abstractions.Scheduling;

public interface IIcdLookupService
{
    Task<IcdDescriptionDto> GetIcdDescriptionAsync(int icdType, string code, CancellationToken cancellationToken = default);
}
