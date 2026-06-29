using SchedulingAdjusterDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.AdjusterDto;
using SchedulingCarrierParentDetailsDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.CarrierParentDetailsDto;
using SchedulingClaimantDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantDto;
using SchedulingClaimantLogDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantLogDto;
using SchedulingClaimantPartialSearchDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantPartialSearchDto;
using SchedulingClaimantSearchResultDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantSearchResultDto;
using SchedulingMergeClaimantDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.MergeClaimantDto;
using SchedulingNurseDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.NurseDto;
using SchedulingMenuItemDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.SchedulingMenuItemDto;
using SchedulingIcdDescriptionDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.IcdDescriptionDto;
using ManageListItemDto = CompX.SharedKernel.Contracts.Manage.Dtos.ManageListItemDto;
using SchedulingOrderSearchColumnPreferencesDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchColumnPreferencesDto;
using SchedulingOrderSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchRowDto;
using CompX.SharedKernel.Contracts.Scheduling.Requests;
using CompX.SharedKernel.Contracts.Scheduling.Dtos;
using CompX.SharedKernel.Contracts.Scheduling.Responses;

namespace CompX.Application.Abstractions.Services;

public interface ISchedulingApplicationService
{
    Task<SchedulingSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(long schedulingId, UpdateSchedulingStatusRequest request, CancellationToken cancellationToken = default);
    Task<SchedulingParityPreviewResponse> PreviewParityAsync(SchedulingParityPreviewRequest request, CancellationToken cancellationToken = default);

    #region---Claimant---
    Task<SchedulingClaimantDto> GetClaimantAsync(int? claimantId, string logon, CancellationToken cancellationToken = default);
    Task<SchedulingClaimantDto> GetClaimantDetailsAsync(int? claimantId, string logon, CancellationToken cancellationToken = default);
    Task<SchedulingClaimantDto> GetDuplicateClaimantAsync(string firstName, string lastName, DateTime? dateOfBirth, string claimNumber, string createdBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingClaimantPartialSearchDto>> GetClaimantByPartialSearchAsync(string claimant, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingClaimantSearchResultDto>> GetClaimantByFilterAsync(string? claimNumber, string? compXNumber, string? searchBy, DateTime? fromDate, DateTime? toDate, string? phone, DateTime? dateOfBirth, CancellationToken cancellationToken = default);
    Task<SchedulingNurseDto> LoadNurseDetailsAsync(int nurseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingNurseDto>> GetNurseBySearchAsync(string searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingMenuItemDto>> GetAllSchedulingMenuItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingClaimantLogDto>> GetClaimantLogAsync(long claimantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetOrderStatusesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingOrderSearchRowDto>> SearchOrderHistoryAsync(OrderSearchRequest request, CancellationToken cancellationToken = default);
    Task<SchedulingOrderSearchColumnPreferencesDto> GetOrderSearchColumnPreferencesAsync(string logonUser, CancellationToken cancellationToken = default);
    Task<SchedulingOrderSearchColumnPreferencesDto> SaveOrderSearchColumnPreferencesAsync(string logonUser, SchedulingOrderSearchColumnPreferencesDto preferences, CancellationToken cancellationToken = default);
    Task<SchedulingClaimantDto> SaveClaimantAsync(SaveClaimantRequest request, string createdOrModifiedBy, CancellationToken cancellationToken = default);
    Task<SchedulingClaimantDto> UploadClaimantAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    Task<SchedulingIcdDescriptionDto> GetIcdDescriptionAsync(int icdType, string code, CancellationToken cancellationToken = default);
    Task<bool> DeleteClaimantAsync(long claimantId, string modifiedBy, CancellationToken cancellationToken = default);
    Task<SchedulingMergeClaimantDto> MergeClaimantAsync(long claimantId, long targetClaimantId, string createdBy, CancellationToken cancellationToken = default);
    Task<SchedulingAdjusterDto> LoadAdjusterDetailsAsync(int adjusterId, CancellationToken cancellationToken = default);
    Task<SchedulingCarrierParentDetailsDto> LoadCarrierChildDetailsAsync(int carrierChildId, CancellationToken cancellationToken = default);
    #endregion
}
