using System.Globalization;
using CompX.Application.Abstractions.Auth;
using CompX.Application.Abstractions.Scheduling;
using CompX.Application.Abstractions.Services;
using SchedulingHistoryScreenDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.SchedulingHistoryScreenDto;
using SchedulingHistoryRequest = CompX.SharedKernel.Contracts.Scheduling.Requests.SchedulingHistory;
using SchedulingOrderSearchColumnPreferencesDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchColumnPreferencesDto;
using SchedulingOrderSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchRowDto;
using SchedulingOrderSearchSourceRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.SchedulingOrderSearchSourceRowDto;

namespace CompX.Application.Services;

public sealed class SchedulingHistory : ISchedulingHistory
{
    private readonly ISchedulingRepository _repo;
    private readonly ICurrentUserContext _currentUser;

    public SchedulingHistory(ISchedulingRepository repo, ICurrentUserContext currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    #region....SchedulingHistory.....

    public async Task<SchedulingOrderSearchColumnPreferencesDto> GetColumnPreferencesDataAsync(string logonUser, CancellationToken cancellationToken = default)
    {
        var user = (logonUser ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(user))
        {
            return new SchedulingOrderSearchColumnPreferencesDto();
        }

        var preferences = await _repo.GetOrderSearchColumnPreferencesAsync(user, cancellationToken);
        if (preferences.ShowHideColumnId != 0)
        {
            return preferences;
        }

        var defaults = BuildDefaultColumnPreferences();
        return await _repo.SaveOrderSearchColumnPreferencesAsync(user, defaults, cancellationToken);
    }

    public async Task<SchedulingHistoryScreenDto> GetSchedulingHistoryAsync(string? orderStatus, string? referrar, CancellationToken cancellationToken = default)
    {
        var logonUser = _currentUser.UserName;
        var adminBypassBilling = _currentUser.IsInRole("Admin");
        var enableProcessedByColumn = _currentUser.IsInRole("Admin");

        var screen = new SchedulingHistoryScreenDto
        {
            OrderStatus = orderStatus,
            Referrar = referrar,
            DepartmentList = await _repo.GetDepartmentListsAsync(cancellationToken),
            AdminCheckBox = adminBypassBilling,
            ByPassForBilling = enableProcessedByColumn
        };

        screen.GetShowColumnsData = await GetColumnPreferencesDataAsync(logonUser, cancellationToken);
        return screen;
    }

    public async Task<IReadOnlyList<SchedulingOrderSearchRowDto>> GetSchedulingOrdersByFilter_ReadAsync(SchedulingHistoryRequest query, CancellationToken cancellationToken = default)
    {
        query ??= new SchedulingHistoryRequest();

        var rows = await _repo.GetOrderSearchSourceRowsAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(query.Department))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.Department, query.Department)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.ServiceType))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.ServiceType, query.ServiceType)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.OrderNo))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.OrderNumberView, query.OrderNo) || ContainsIgnoreCase(x.OrderNumber, query.OrderNo)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Jurisdiction))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.JurisdictionCode, query.Jurisdiction) || ContainsIgnoreCase(x.Jurisdiction, query.Jurisdiction)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Claimant))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.ClaimantName, query.Claimant)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Provider))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.VendorName, query.Provider)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Referrar))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.Referrar, query.Referrar)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Payer))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.PayerName, query.Payer)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.OrderStatus))
        {
            rows = rows.Where(x => ContainsIgnoreCase(x.OrderStatusWithReason, query.OrderStatus) || ContainsIgnoreCase(x.OrderStatus, query.OrderStatus)).ToList();
        }

        if (!query.ShowInactiveProviders)
        {
            rows = rows.Where(x => !IsInactiveProvider(x.VendorStatusName, x.VendorStatus)).ToList();
        }

        if (query.FromApptDate.HasValue || query.ToApptDate.HasValue)
        {
            rows = rows.Where(x =>
            {
                var appointmentDate = ParseDate(x.AppointmentDateStr) ?? ParseDate(x.AppointmentDate);
                if (!appointmentDate.HasValue)
                {
                    return false;
                }

                if (query.FromApptDate.HasValue && appointmentDate.Value.Date < query.FromApptDate.Value.Date)
                {
                    return false;
                }

                if (query.ToApptDate.HasValue && appointmentDate.Value.Date > query.ToApptDate.Value.Date)
                {
                    return false;
                }

                return true;
            }).ToList();
        }

        return rows
            .OrderByDescending(x => x.CreatedDate ?? DateTime.MinValue)
            .Select(MapRow)
            .ToList();
    }

    private static SchedulingOrderSearchRowDto MapRow(SchedulingOrderSearchSourceRowDto x)
    {
        return new SchedulingOrderSearchRowDto
        {
            ProcessedBy = x.ByPassForBilling == true ? "Integration" : string.Empty,
            Claimant = x.ClaimantName,
            Dob = null,
            ServiceType = x.ServiceType,
            Provider = x.VendorName,
            Referrar = x.Referrar,
            ProviderStatus = x.VendorStatusName,
            Payer = x.PayerName,
            ReferralDate = FormatDate(x.ReferralDate),
            ReferralPhone = x.ReferralPhone,
            AppointmentDate = x.AppointmentDateStr ?? x.AppointmentDate,
            AppointmentTime = x.AppointmentTimeStr ?? x.AppointmentTime,
            DestinationName = x.DestinationName,
            AssignedTo = x.AssignedTo,
            CreatedDate = FormatDate(x.CreatedDate),
            OrderStatus = x.OrderStatusWithReason ?? x.OrderStatus,
            InvoiceStatus = x.BillStatus,
            Jurisdiction = x.JurisdictionCode ?? x.Jurisdiction,
            SchedulingId = TryParseLong(x.SchedulingId),
            SchedulingChildId = TryParseLong(x.SchedulingChildId),
            OrderNumber = x.OrderNumber,
            OrderNumberView = x.OrderNumberView,
            DepartmentId = x.DepartmentId,
            ByPassForBilling = x.ByPassForBilling
        };
    }

    private static bool ContainsIgnoreCase(string? source, string term)
        => !string.IsNullOrWhiteSpace(source) &&
           !string.IsNullOrWhiteSpace(term) &&
           source.Contains(term, StringComparison.OrdinalIgnoreCase);

    private static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static long? TryParseLong(string? value)
        => long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;

    private static string? FormatDate(DateTime? value)
        => value.HasValue ? value.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : null;

    private static bool IsInactiveProvider(string? vendorStatusName, string? vendorStatus)
    {
        var status = (vendorStatusName ?? vendorStatus ?? string.Empty).Trim();
        return string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(status, "1", StringComparison.OrdinalIgnoreCase);
    }

    private static SchedulingOrderSearchColumnPreferencesDto BuildDefaultColumnPreferences()
    {
        return new SchedulingOrderSearchColumnPreferencesDto
        {
            ClaimantName = true,
            ClaimantDob = true,
            ServiceType = true,
            VendorName = true,
            VendorStatusName = true,
            PayerName = true,
            ReferralDate = true,
            ReferralPhone = true,
            AppointmentDate = true,
            AppointmentTimeStr = true,
            DestinationName = true,
            AssignedTo = true,
            CreatedDate = true,
            OrderStatusWithReason = true,
            BillStatus = true,
            Jurisdiction = true
        };
    }

    #endregion
}
