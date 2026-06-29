using CompX.Domain.Billing;
using CompX.SharedKernel.Contracts.Common.Dtos;

namespace CompX.Application.Mappings;

public static class BillingContractMapper
{
    public static BillingInvoiceStatus ToDomainStatus(BillingInvoiceStatusContract status) => status switch
    {
        BillingInvoiceStatusContract.None => BillingInvoiceStatus.New,
        BillingInvoiceStatusContract.Ready => BillingInvoiceStatus.Ready,
        BillingInvoiceStatusContract.Billed => BillingInvoiceStatus.Billed,
        BillingInvoiceStatusContract.Cancelled => BillingInvoiceStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported billing invoice status.")
    };

    public static BillingInvoiceStatusContract ToContractStatus(BillingInvoiceStatus status) => status switch
    {
        BillingInvoiceStatus.New => BillingInvoiceStatusContract.None,
        BillingInvoiceStatus.Ready => BillingInvoiceStatusContract.Ready,
        BillingInvoiceStatus.Billed => BillingInvoiceStatusContract.Billed,
        BillingInvoiceStatus.Cancelled => BillingInvoiceStatusContract.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported billing invoice status.")
    };
}
