using CompX.Application.Abstractions.Billing;
using CompX.Application.Abstractions.Persistence;
using CompX.Application.Abstractions.Services;
using CompX.Application.Mappings;
using CompX.Domain.Billing;
using CompX.SharedKernel.Contracts.Billing.Dtos;
using CompX.SharedKernel.Contracts.Billing.Requests;
using CompX.SharedKernel.Contracts.Billing.Responses;

namespace CompX.Application.Services;

public sealed class BillingApplicationService : IBillingApplicationService
{
    private readonly IBillingRepository _repo;
    private readonly IUnitOfWork _uow;

    public BillingApplicationService(IBillingRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public Task<BillingSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        => _repo.GetSummaryAsync(cancellationToken);

    public async Task<bool> UpdateTransportationStatusAsync(long billingTransportationId, UpdateBillingStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy)) throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        var invoice = await _repo.GetTransportationInvoiceAsync(billingTransportationId, cancellationToken);
        if (invoice is null) return false;

        var docSatisfied = BillingDocumentPolicy.IsProviderBillSatisfied(request.HasProviderBill, request.HasApprovedDocument);
        var next = BillingStatusPolicy.ResolveNextStatus(invoice.CurrentStatus, docSatisfied, request.IsCancelled);
        var changed = invoice.TryTransition(next);
        if (!changed) return true;

        await _repo.UpdateTransportationInvoiceStatusAsync(invoice.InvoiceId, invoice.CurrentStatus, invoice.PreviousStatus, invoice.IsDeleted, request.ModifiedBy, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<BillingParityPreviewResponse> PreviewParityAsync(BillingParityPreviewRequest request, CancellationToken cancellationToken = default)
    {
        var eligible = BillingEligibilityPolicy.IsEligible(request.IsCancelled, request.ByPassForBilling, request.IsAdmin, request.OrderStatus);
        var docSatisfied = BillingDocumentPolicy.IsProviderBillSatisfied(request.HasProviderBill, request.HasApprovedDocument);
        var next = BillingStatusPolicy.ResolveNextStatus(BillingContractMapper.ToDomainStatus(request.CurrentStatus), docSatisfied, request.IsCancelled);
        var lookup = await _repo.GetCancellationReasonLookupAsync(cancellationToken);
        var reason = BillingCancellationReasonPolicy.ResolveReason(request.CancellationReasonId, request.CancellationReasonOther, lookup);
        return new BillingParityPreviewResponse(eligible, BillingContractMapper.ToContractStatus(next), reason);
    }
}
