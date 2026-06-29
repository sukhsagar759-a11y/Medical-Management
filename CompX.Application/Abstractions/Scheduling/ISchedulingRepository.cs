using SchedulingAdjusterDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.AdjusterDto;
using SchedulingCarrierParentDetailsDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.CarrierParentDetailsDto;
using SchedulingClaimantDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantDto;
using SchedulingClaimantLogDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantLogDto;
using SchedulingClaimantLogRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantLogRowDto;
using SchedulingClaimantPartialSearchDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantPartialSearchDto;
using SchedulingClaimantSearchResultDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantSearchResultDto;
using SchedulingClaimantSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantSearchRowDto;
using SchedulingMergeClaimantDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.MergeClaimantDto;
using SchedulingNurseDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.NurseDto;
using SchedulingNurseSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.NurseSearchRowDto;
using SchedulingMenuItemDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.SchedulingMenuItemDto;
using ManageListItemDto = CompX.SharedKernel.Contracts.Manage.Dtos.ManageListItemDto;
using SchedulingOrderSearchColumnPreferencesDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchColumnPreferencesDto;
using SchedulingOrderSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchRowDto;
using SchedulingOrderSearchSourceRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.SchedulingOrderSearchSourceRowDto;
using SchedulingOrderSearchRequest = CompX.SharedKernel.Contracts.Scheduling.Requests.OrderSearchRequest;
using CompX.SharedKernel.Contracts.Scheduling.Dtos;
using CompX.Domain.Scheduling;

namespace CompX.Application.Abstractions.Scheduling;

public interface ISchedulingRepository
{
    Task<SchedulingSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<SchedulingOrder?> GetByIdAsync(long schedulingId, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(long schedulingId, SchedulingOrderStatus status, bool isCancelled, string modifiedBy, CancellationToken cancellationToken = default);

    #region---Claimant---
    Task<SchedulingClaimantDto> GetClaimantAsync(int claimantId, string logon, CancellationToken cancellationToken = default);
    Task<SchedulingClaimantDto> GetDuplicateClaimantAsync(string firstName, string lastName, DateTime dateOfBirth, string claimNumber, string createdBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingClaimantPartialSearchDto>> GetClaimantByPartialSearchAsync(string claimant, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingClaimantSearchRowDto>> GetClaimantByFilterAsync(string? claimNumber, string? compXNumber, string? searchBy, DateTime? fromDate, DateTime? toDate, string? phone, DateTime? dateOfBirth, CancellationToken cancellationToken = default);
    Task<SchedulingNurseDto> LoadNurseDetailsAsync(int nurseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingNurseSearchRowDto>> GetNurseBySearchAsync(string searchValue, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingMenuItemDto>> GetAllSchedulingMenuItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingClaimantLogRowDto>> GetClaimantLogAsync(long claimantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetOrderStatusesAsync(CancellationToken cancellationToken = default);

    #region....SchedulingHistory.....

    Task<IReadOnlyList<SchedulingOrderSearchSourceRowDto>> GetOrderSearchSourceRowsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingOrderSearchRowDto>> SearchOrderHistoryAsync(SchedulingOrderSearchRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManageListItemDto>> GetDepartmentListsAsync(CancellationToken cancellationToken = default);
    Task<SchedulingOrderSearchColumnPreferencesDto> GetOrderSearchColumnPreferencesAsync(string logonUser, CancellationToken cancellationToken = default);
    Task<SchedulingOrderSearchColumnPreferencesDto> SaveOrderSearchColumnPreferencesAsync(string logonUser, SchedulingOrderSearchColumnPreferencesDto preferences, CancellationToken cancellationToken = default);

    #endregion

    Task<SchedulingClaimantDto> SaveClaimantAsync(SaveClaimantRequest request, string createdOrModifiedBy, CancellationToken cancellationToken = default);
    Task<bool> DeleteClaimantAsync(long claimantId, string modifiedBy, CancellationToken cancellationToken = default);
    Task<SchedulingMergeClaimantDto> MergeClaimantAsync(long claimantId, long targetClaimantId, string createdBy, CancellationToken cancellationToken = default);
    Task<SchedulingAdjusterDto> LoadAdjusterDetailsAsync(int adjusterId, CancellationToken cancellationToken = default);
    Task<SchedulingCarrierParentDetailsDto> LoadCarrierChildDetailsAsync(int carrierChildId, CancellationToken cancellationToken = default);
    #endregion
}
