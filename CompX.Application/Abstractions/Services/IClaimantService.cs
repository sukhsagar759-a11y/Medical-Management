using CompX.SharedKernel.Contracts.Scheduling.Dtos;

namespace CompX.Application.Abstractions.Services
{
    public interface IClaimantService
    {
        Task<ClaimantDto?> GetClaimantDetailsAsync(
            string? logon,
            int claimantId,
            CancellationToken cancellationToken = default);
    }
}
