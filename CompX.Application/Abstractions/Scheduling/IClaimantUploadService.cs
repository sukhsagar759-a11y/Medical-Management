using ClaimantUploadDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantDto;

namespace CompX.Application.Abstractions.Scheduling;

public interface IClaimantUploadService
{
    Task<ClaimantUploadDto> UploadClaimantAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}
