using CompX.Domain.Scheduling;
using CompX.SharedKernel.Contracts.Common.Dtos;

namespace CompX.Application.Mappings;

public static class SchedulingContractMapper
{
    public static SchedulingOrderStatus ToDomainStatus(SchedulingOrderStatusContract status) => status switch
    {
        SchedulingOrderStatusContract.New => SchedulingOrderStatus.New,
        SchedulingOrderStatusContract.Scheduled => SchedulingOrderStatus.Scheduled,
        SchedulingOrderStatusContract.Complete => SchedulingOrderStatus.Complete,
        SchedulingOrderStatusContract.Cancelled => SchedulingOrderStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported scheduling status.")
    };

    public static SchedulingTaskTypeContract ToContractTask(SchedulingTaskType taskType) => taskType switch
    {
        SchedulingTaskType.None => SchedulingTaskTypeContract.None,
        SchedulingTaskType.ObtainAuthorization => SchedulingTaskTypeContract.ObtainAuthorization,
        SchedulingTaskType.PlaceOrder => SchedulingTaskTypeContract.PlaceOrder,
        SchedulingTaskType.ConfirmDelivery => SchedulingTaskTypeContract.ConfirmDelivery,
        SchedulingTaskType.UploadProviderBill => SchedulingTaskTypeContract.UploadProviderBill,
        _ => throw new ArgumentOutOfRangeException(nameof(taskType), taskType, "Unsupported scheduling task type.")
    };
}
