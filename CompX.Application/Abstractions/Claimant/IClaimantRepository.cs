using CompX.SharedKernel.Contracts.Scheduling.Dtos;

namespace CompX.Application.Abstractions.Claimant
{
    public interface IClaimantRepository
    {
        Task<ClaimantDto?> GetClaimantDetailsAsync(
            string logon,
            int claimantId,
            CancellationToken cancellationToken = default);
    }
}
