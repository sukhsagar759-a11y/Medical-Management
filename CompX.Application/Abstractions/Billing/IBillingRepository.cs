using CompX.SharedKernel.Contracts.Billing.Dtos;
using CompX.Domain.Billing;

namespace CompX.Application.Abstractions.Billing;

public interface IBillingRepository
{
    Task<BillingSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<byte, string>> GetCancellationReasonLookupAsync(CancellationToken cancellationToken = default);
    Task<BillingInvoice?> GetTransportationInvoiceAsync(long billingTransportationId, CancellationToken cancellationToken = default);
    Task UpdateTransportationInvoiceStatusAsync(long billingTransportationId, BillingInvoiceStatus status, BillingInvoiceStatus? previousStatus, bool deleted, string modifiedBy, CancellationToken cancellationToken = default);
}
