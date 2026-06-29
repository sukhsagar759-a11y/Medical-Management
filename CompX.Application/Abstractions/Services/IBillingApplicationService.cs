using CompX.SharedKernel.Contracts.Billing.Dtos;
using CompX.SharedKernel.Contracts.Billing.Requests;
using CompX.SharedKernel.Contracts.Billing.Responses;

namespace CompX.Application.Abstractions.Services;

public interface IBillingApplicationService
{
    Task<BillingSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateTransportationStatusAsync(long billingTransportationId, UpdateBillingStatusRequest request, CancellationToken cancellationToken = default);
    Task<BillingParityPreviewResponse> PreviewParityAsync(BillingParityPreviewRequest request, CancellationToken cancellationToken = default);
}
