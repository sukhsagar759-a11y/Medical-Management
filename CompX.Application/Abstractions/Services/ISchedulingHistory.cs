using SchedulingHistoryRequest = CompX.SharedKernel.Contracts.Scheduling.Requests.SchedulingHistory;
using SchedulingHistoryScreenDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.SchedulingHistoryScreenDto;
using SchedulingOrderSearchColumnPreferencesDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchColumnPreferencesDto;
using SchedulingOrderSearchRowDto = CompX.SharedKernel.Contracts.Scheduling.Dtos.OrderSearchRowDto;

namespace CompX.Application.Abstractions.Services;

public interface ISchedulingHistory
{
    #region....SchedulingHistory.....

    Task<SchedulingOrderSearchColumnPreferencesDto> GetColumnPreferencesDataAsync(string logonUser, CancellationToken cancellationToken = default);
    Task<SchedulingHistoryScreenDto> GetSchedulingHistoryAsync(string? orderStatus, string? referrar, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchedulingOrderSearchRowDto>> GetSchedulingOrdersByFilter_ReadAsync(SchedulingHistoryRequest query, CancellationToken cancellationToken = default);

    #endregion
}
