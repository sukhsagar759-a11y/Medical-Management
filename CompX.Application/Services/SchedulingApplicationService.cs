using SchedulingAdjusterDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.AdjusterDto;
using SchedulingCarrierParentDetailsDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.CarrierParentDetailsDto;
using SchedulingClaimantDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantDto;
using SchedulingClaimantLogDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantLogDto;
using SchedulingClaimantPartialSearchDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantPartialSearchDto;
using SchedulingClaimantSearchResultDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantSearchResultDto;
using SchedulingClaimantSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantSearchRowDto;
using SchedulingIcdDescriptionDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.IcdDescriptionDto;
using SchedulingMergeClaimantDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.MergeClaimantDto;
using SchedulingNurseDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.NurseDto;
using SchedulingNurseSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.NurseSearchRowDto;
using SchedulingMenuItemDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.SchedulingMenuItemDto;
using SchedulingClaimantLogRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.ClaimantLogRowDto;
using SchedulingOrderSearchColumnPreferencesDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchColumnPreferencesDto;
using SchedulingOrderSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchRowDto;
using ManageListItemDto = CompX.SharedKernel.Contracts.Manage.Dtos.ManageListItemDto;
using CompX.Application.Abstractions.Persistence;
using CompX.Application.Abstractions.Scheduling;
using CompX.Application.Abstractions.Services;
using CompX.Domain.Scheduling;
using CompX.Application.Mappings;
using CompX.SharedKernel.Contracts.Scheduling.Dtos;
using CompX.SharedKernel.Contracts.Scheduling.Requests;
using CompX.SharedKernel.Contracts.Scheduling.Responses;

namespace CompX.Application.Services;

public sealed class SchedulingApplicationService : ISchedulingApplicationService
{
    private readonly ISchedulingRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IClaimantUploadService _claimantUploadService;
    private readonly IIcdLookupService _icdLookupService;

    public SchedulingApplicationService(ISchedulingRepository repo, IUnitOfWork uow, IClaimantUploadService claimantUploadService, IIcdLookupService icdLookupService)
    {
        _repo = repo;
        _uow = uow;
        _claimantUploadService = claimantUploadService;
        _icdLookupService = icdLookupService;
    }

    public Task<SchedulingSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        => _repo.GetSummaryAsync(cancellationToken);

    public async Task<bool> UpdateStatusAsync(long schedulingId, UpdateSchedulingStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy)) throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        var order = await _repo.GetByIdAsync(schedulingId, cancellationToken);
        if (order is null) return false;

        var changed = request.IsCancelled ? order.MarkCancelled() : order.ChangeStatus(SchedulingContractMapper.ToDomainStatus(request.Status));
        if (!changed) return true;

        await _repo.UpdateStatusAsync(order.SchedulingId, order.Status, order.IsCancelled, request.ModifiedBy, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<SchedulingParityPreviewResponse> PreviewParityAsync(SchedulingParityPreviewRequest request, CancellationToken cancellationToken = default)
    {
        var order = new SchedulingOrder(1, "PARITY-PREVIEW", SchedulingContractMapper.ToDomainStatus(request.Status), request.IsCancelled, request.ByPassForBilling);
        var nextTask = SchedulingTaskPolicy.SelectNextTask(new SchedulingTaskContext(
            request.AuthorizationRequired,
            request.IsAuthorized,
            request.HasSupplier,
            request.IsOrdered,
            request.HasAppointment,
            request.IsAppointmentCompleted,
            request.IsProviderBillReceived));

        return Task.FromResult(new SchedulingParityPreviewResponse(order.CanMoveToBilling(), SchedulingContractMapper.ToContractTask(nextTask)));
    }

    #region---Claimant---

    public Task<SchedulingClaimantDto> GetClaimantAsync(int? claimantId, string logon, CancellationToken cancellationToken = default)
        => claimantId is null || claimantId <= 0
            ? Task.FromResult(new SchedulingClaimantDto { IsEmailNotification = false })
            : _repo.GetClaimantAsync(claimantId.Value, logon, cancellationToken);

    public Task<SchedulingClaimantDto> GetClaimantDetailsAsync(int? claimantId, string logon, CancellationToken cancellationToken = default)
        => claimantId is null || claimantId <= 0
            ? Task.FromResult(new SchedulingClaimantDto { IsEmailNotification = false })
            : _repo.GetClaimantAsync(claimantId.Value, logon, cancellationToken);

    public Task<SchedulingClaimantDto> GetDuplicateClaimantAsync(string firstName, string lastName, DateTime? dateOfBirth, string claimNumber, string createdBy, CancellationToken cancellationToken = default)
        => _repo.GetDuplicateClaimantAsync(firstName, lastName, dateOfBirth ?? DateTime.Today, claimNumber, createdBy, cancellationToken);

    public Task<IReadOnlyList<SchedulingClaimantPartialSearchDto>> GetClaimantByPartialSearchAsync(string claimant, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(claimant)
            ? Task.FromResult<IReadOnlyList<SchedulingClaimantPartialSearchDto>>(Array.Empty<SchedulingClaimantPartialSearchDto>())
            : _repo.GetClaimantByPartialSearchAsync(claimant.Trim(), cancellationToken);

    public async Task<IReadOnlyList<SchedulingClaimantSearchResultDto>> GetClaimantByFilterAsync(string? claimNumber, string? compXNumber, string? searchBy, DateTime? fromDate, DateTime? toDate, string? phone, DateTime? dateOfBirth, CancellationToken cancellationToken = default)
    {
        var rows = await _repo.GetClaimantByFilterAsync(claimNumber, compXNumber, searchBy, fromDate, toDate, phone, dateOfBirth, cancellationToken);
        return rows.Select(x =>
        {
            var claimantName = BuildClaimantName(x.LastName, x.FirstName);
            return new SchedulingClaimantSearchResultDto
            {
                ClaimantId = x.ClaimantId,
                ClaimNumber = x.ClaimNumber,
                PolicyNumber = x.PolicyNumber,
                CompXNumber = x.CompXNumber,
                LastName = x.LastName,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                ClaimantName = claimantName,
                Sex = x.Sex,
                DateOfBirth = x.DateOfBirth,
                SSN = x.SSN,
                Address1 = x.Address1,
                Address2 = x.Address2,
                Zip = x.Zip,
                City = x.City,
                County = x.County,
                State = x.State,
                Phone = x.Phone,
                AlternatePhone = x.AlternatePhone,
                Email = x.Email,
                Height = x.Height,
                Weight = x.Weight,
                Employer = x.Employer,
                Jurisdiction = x.Jurisdiction,
                JuriName = x.JuriName,
                DateOfInjury = x.DateOfInjury,
                CompanyId = x.CompanyId,
                CompanyName = x.CompanyName,
                NurseId = x.NurseId,
                NurseName = x.NurseName,
                AdjusterId = x.AdjusterId,
                AdjusterName = x.AdjusterName,
                CarrierParentId = x.CarrierParentId,
                ParentName = x.ParentName,
                CarrierChildId = x.CarrierChildId,
                ChildName = x.ChildName,
                BenifitEffectiveDate = x.BenifitEffectiveDate,
                ICDType = x.ICDType,
                ICD1 = x.ICD1,
                ICD2 = x.ICD2,
                ICD3 = x.ICD3,
                ICD4 = x.ICD4,
                ICD5 = x.ICD5,
                IsAttorney = x.IsAttorney,
                AttorneyComments = x.AttorneyComments
            };
        }).ToList();
    }

    public Task<SchedulingNurseDto> LoadNurseDetailsAsync(int nurseId, CancellationToken cancellationToken = default)
        => _repo.LoadNurseDetailsAsync(nurseId, cancellationToken);

    public async Task<IReadOnlyList<SchedulingNurseDto>> GetNurseBySearchAsync(string searchValue, CancellationToken cancellationToken = default)
    {
        var rows = await _repo.GetNurseBySearchAsync(searchValue, cancellationToken);
        return rows
            .Select(x =>
            {
                var companyName = x.CompanyName ?? x.CarrierName;
                var displayName = BuildNurseCompanyName(x.Name, companyName);
                return new SchedulingNurseDto
                {
                    NurseId = x.NurseId,
                    CompanyId = x.CompanyId,
                    CarrierId = x.CarrierId,
                    Name = x.Name,
                    Phone = x.Phone,
                    Email = x.Email,
                    Fax = null,
                    Notes = x.Notes,
                    ComapnyName = companyName,
                    NurseCompanyName = displayName
                };
            })
            .Where(x => string.IsNullOrWhiteSpace(searchValue) || (x.NurseCompanyName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.NurseCompanyName)
            .ToList();
    }

    public Task<IReadOnlyList<SchedulingMenuItemDto>> GetAllSchedulingMenuItemsAsync(CancellationToken cancellationToken = default)
        => _repo.GetAllSchedulingMenuItemsAsync(cancellationToken);

    public Task<IReadOnlyList<ManageListItemDto>> GetOrderStatusesAsync(CancellationToken cancellationToken = default)
        => _repo.GetOrderStatusesAsync(cancellationToken);

    public Task<IReadOnlyList<SchedulingOrderSearchRowDto>> SearchOrderHistoryAsync(OrderSearchRequest request, CancellationToken cancellationToken = default)
        => _repo.SearchOrderHistoryAsync(request ?? new OrderSearchRequest(), cancellationToken);

    public Task<SchedulingOrderSearchColumnPreferencesDto> GetOrderSearchColumnPreferencesAsync(string logonUser, CancellationToken cancellationToken = default)
        => _repo.GetOrderSearchColumnPreferencesAsync(logonUser, cancellationToken);

    public Task<SchedulingOrderSearchColumnPreferencesDto> SaveOrderSearchColumnPreferencesAsync(string logonUser, SchedulingOrderSearchColumnPreferencesDto preferences, CancellationToken cancellationToken = default)
        => _repo.SaveOrderSearchColumnPreferencesAsync(logonUser, preferences, cancellationToken);

    public async Task<IReadOnlyList<SchedulingClaimantLogDto>> GetClaimantLogAsync(long claimantId, CancellationToken cancellationToken = default)
    {
        var rows = await _repo.GetClaimantLogAsync(claimantId, cancellationToken);
        return rows.Select(x => new SchedulingClaimantLogDto
        {
            Action = x.Operation == SchedulingAuditConstants.OperationAdd ? "Add" :
                     x.Operation == SchedulingAuditConstants.OperationEdit ? "Edit" :
                     x.Operation == SchedulingAuditConstants.OperationDelete ? "Delete" :
                     x.Operation == SchedulingAuditConstants.OperationRead ? "Read" : string.Empty,
            User = $"{x.UserLastName}, {x.UserFirstName}".Trim().Trim(','),
            CreatedDateTime = x.CreatedDateTime,
            DateTime = x.CreatedDateTime?.ToString()
        }).OrderByDescending(x => x.CreatedDateTime).ToList();
    }

    public Task<SchedulingClaimantDto> SaveClaimantAsync(SaveClaimantRequest request, string createdOrModifiedBy, CancellationToken cancellationToken = default)
        => _repo.SaveClaimantAsync(request, createdOrModifiedBy, cancellationToken);

    public Task<SchedulingClaimantDto> UploadClaimantAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        => _claimantUploadService.UploadClaimantAsync(fileStream, fileName, cancellationToken);

    public Task<SchedulingIcdDescriptionDto> GetIcdDescriptionAsync(int icdType, string code, CancellationToken cancellationToken = default)
        => _icdLookupService.GetIcdDescriptionAsync(icdType, code, cancellationToken);

    public Task<bool> DeleteClaimantAsync(long claimantId, string modifiedBy, CancellationToken cancellationToken = default)
        => _repo.DeleteClaimantAsync(claimantId, modifiedBy, cancellationToken);

    public Task<SchedulingMergeClaimantDto> MergeClaimantAsync(long claimantId, long targetClaimantId, string createdBy, CancellationToken cancellationToken = default)
        => _repo.MergeClaimantAsync(claimantId, targetClaimantId, createdBy, cancellationToken);

    public Task<SchedulingAdjusterDto> LoadAdjusterDetailsAsync(int adjusterId, CancellationToken cancellationToken = default)
        => _repo.LoadAdjusterDetailsAsync(adjusterId, cancellationToken);

    public Task<SchedulingCarrierParentDetailsDto> LoadCarrierChildDetailsAsync(int carrierChildId, CancellationToken cancellationToken = default)
        => _repo.LoadCarrierChildDetailsAsync(carrierChildId, cancellationToken);

    #endregion

    private static string BuildClaimantName(string? lastName, string? firstName)
    {
        var name = $"{lastName}, {firstName}".Trim();
        return name.Trim(',', ' ');
    }

    private static string BuildNurseCompanyName(string? nurseName, string? companyName)
    {
        var name = (nurseName ?? string.Empty).Split(new[] { "##" }, StringSplitOptions.RemoveEmptyEntries);
        var displayName = name.Length > 1
            ? $"{name[0]} {name[1]}"
            : name.Length > 0 ? name[0] : string.Empty;

        return string.IsNullOrWhiteSpace(companyName)
            ? displayName
            : $"{displayName} - {companyName}";
    }
}
