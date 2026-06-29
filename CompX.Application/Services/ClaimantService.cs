using CompX.Application.Abstractions.Claimant;
using CompX.Application.Abstractions.Services;
using CompX.SharedKernel.Contracts.Scheduling.Dtos;

namespace CompX.Application.Services
{
    public class ClaimantService : IClaimantService
    {
        private readonly IClaimantRepository repository;
        private readonly ICommonService commonService;

        public ClaimantService(
            IClaimantRepository repository,
            ICommonService commonService)
        {
            this.repository = repository;
            this.commonService = commonService;
        }

        public async Task<ClaimantDto?> GetClaimantDetailsAsync(
            string? logon,
            int claimantId,
            CancellationToken cancellationToken = default)
        {
            var model = await repository.GetClaimantDetailsAsync(
                logon ?? string.Empty,
                claimantId,
                cancellationToken);

            if (model is null)
                return null;

            SetHeightParts(model);
            SetNurseCompanyName(model);

            return model;
        }

        private static void SetHeightParts(ClaimantDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Height))
                return;

            var heightParts = model.Height
                .Replace("\"", string.Empty)
                .Split('\'', StringSplitOptions.TrimEntries);

            model.Feet = heightParts.Length > 0 ? heightParts[0] : string.Empty;
            model.Inches = heightParts.Length > 1 ? heightParts[1] : string.Empty;
        }

        private void SetNurseCompanyName(ClaimantDto model)
        {
            if (string.IsNullOrWhiteSpace(model.NurseName))
                return;

            var nurseName = commonService.GetPhysicianName(model.NurseName);

            model.NurseCompanyName = string.IsNullOrWhiteSpace(model.CompanyName)
                ? nurseName
                : $"{nurseName} - {model.CompanyName}";
        }
    }
}
