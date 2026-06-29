using CompX.Application.Abstractions.AccountReceivable;
using CompX.Application.Abstractions.Persistence;
using CompX.Application.Abstractions.Services;
using CompX.Application.Mappings;
using CompX.Domain.AccountReceivable;
using CompX.SharedKernel.Contracts.AccountReceivable.Dtos;
using CompX.SharedKernel.Contracts.AccountReceivable.Requests;
using CompX.SharedKernel.Contracts.AccountReceivable.Responses;

namespace CompX.Application.Services;

public sealed class AccountReceivableApplicationService : IAccountReceivableApplicationService
{
    private readonly IAccountReceivableRepository _repo;
    private readonly IUnitOfWork _uow;

    public AccountReceivableApplicationService(
        IAccountReceivableRepository repo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public Task<AccountReceivableSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        => _repo.GetSummaryAsync(cancellationToken);

    public Task<IReadOnlyList<AccountReceivableLedgerItemDto>> GetByInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken = default)
        => _repo.GetByInvoiceAsync(invoiceNumber, cancellationToken);

    public Task<IReadOnlyList<AccountReceivableInvoiceOptionDto>> SearchInvoicesAsync(string invoiceLike, CancellationToken cancellationToken = default)
        => _repo.SearchInvoicesAsync(invoiceLike, cancellationToken);

    public Task<AccountReceivableAmountDueDetailsDto> GetAmountDueDetailsByInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken = default)
        => _repo.GetAmountDueDetailsByInvoiceAsync(invoiceNumber, cancellationToken);

    public Task<IReadOnlyList<AdjustmentCodeDto>> GetAdjustmentCodesAsync(CancellationToken cancellationToken = default)
        => _repo.GetAdjustmentCodesAsync(cancellationToken);

    public Task<IReadOnlyList<AccountReceivableBatchLookupDto>> SearchBatchNumbersAsync(string searchValue, CancellationToken cancellationToken = default)
        => _repo.SearchBatchNumbersAsync(searchValue, cancellationToken);

    public Task<AccountReceivableBatchDetailsDto?> GetBatchDetailsAsync(string batchNumber, CancellationToken cancellationToken = default)
        => _repo.GetBatchDetailsAsync(batchNumber, cancellationToken);

    public Task<AccountReceivableBatchDetailsDto> GetBatchPaymentsAsync(int batchId, CancellationToken cancellationToken = default)
        => _repo.GetBatchPaymentsAsync(batchId, cancellationToken);

    public Task<AccountReceivableDraftSnapshotDto> GetDraftAsync(string userKey, CancellationToken cancellationToken = default)
        => _repo.GetDraftAsync(userKey, cancellationToken);

    public async Task<AccountReceivableDraftSnapshotDto> AddDraftGridRowAsync(string userKey, AddAccountReceivableGridRowRequest request, CancellationToken cancellationToken = default)
    {
        var snapshot = await _repo.AddDraftGridRowAsync(userKey, request, cancellationToken);
        var shouldCommit = request.BatchId.GetValueOrDefault() > 0 || string.Equals(request.Transactions, "Single", StringComparison.OrdinalIgnoreCase);
        if (!shouldCommit)
        {
            await _uow.SaveChangesAsync(cancellationToken);
            return snapshot;
        }

        foreach (var row in snapshot.Rows)
        {
            var batchRequest = new UpsertBatchPaymentRequest(
                null,
                request.BatchId ?? request.SingleBatchId,
                row.InvoiceNumber,
                row.AmountDue,
                row.AmountPaid,
                row.AdjustmentAmount,
                row.AdjustmentCode,
                row.CheckDate,
                row.CheckNumber,
                row.Memo,
                row.Payer,
                row.TransactionType,
                row.TransactionDate,
                row.DocumentType,
                row.DocumentName,
                row.Review,
                request.ModifiedBy);
            await _repo.UpsertBatchPaymentAsync(batchRequest, cancellationToken);
        }

        var invoiceNumbers = snapshot.Rows
            .Select(x => x.InvoiceNumber.Trim().ToUpperInvariant())
            .Where(x => x.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var isSingleTransaction = string.Equals(request.Transactions, "Single", StringComparison.OrdinalIgnoreCase);
        var previousStatusBatchId = isSingleTransaction ? null : request.BatchId;
        await _repo.ApplySaveAccountReceivableSideEffectsAsync(invoiceNumbers, previousStatusBatchId, request.ModifiedBy, cancellationToken);

        await _repo.ClearDraftAsync(userKey, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return new AccountReceivableDraftSnapshotDto(userKey, 0, 0m, 0m, []);
    }

    public async Task<AccountReceivableDraftSnapshotDto> UpdateDraftGridRowAsync(string userKey, UpdateDraftRowRequest request, CancellationToken cancellationToken = default)
    {
        var snapshot = await _repo.UpdateDraftGridRowAsync(userKey, request, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return snapshot;
    }

    public async Task<AccountReceivableDraftSnapshotDto> DeleteDraftGridRowAsync(string userKey, int rowId, CancellationToken cancellationToken = default)
    {
        var snapshot = await _repo.DeleteDraftGridRowAsync(userKey, rowId, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return snapshot;
    }

    public async Task<BatchOperationResponse> AccountsAsync(string userKey, SaveBatchRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy))
        {
            throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        }

        var effectiveBatchStatus = AccountReceivableBatchPolicy.ResolveBatchStatus(request.SubmitType, request.BatchStatus);
        if (AccountReceivableBatchPolicy.IsZeroDepositInvalid(request.BatchDepositTotal, request.TransactionType))
        {
            var batchLabel = string.IsNullOrWhiteSpace(request.BatchNumber) ? "Batch" : request.BatchNumber;
            return new BatchOperationResponse(
                false,
                request.BatchId,
                request.BatchNumber,
                $"{batchLabel} - Batch deposit total must be greater than zero for this transaction type.");
        }

        var effectiveRequest = request with { BatchStatus = effectiveBatchStatus };
        var draft = await _repo.GetDraftAsync(userKey, cancellationToken);

        // Legacy parity: if staged grid rows exist, persist them with batch details and clear draft.
        if (draft.Rows.Count > 0)
        {
            var committedBatchId = await _repo.AccountsSaveAsync(effectiveRequest, cancellationToken);

            foreach (var row in draft.Rows)
            {
                var rowRequest = new UpsertBatchPaymentRequest(
                    null,
                    committedBatchId,
                    row.InvoiceNumber,
                    row.AmountDue,
                    row.AmountPaid,
                    row.AdjustmentAmount,
                    row.AdjustmentCode,
                    row.CheckDate,
                    row.CheckNumber,
                    row.Memo,
                    row.Payer,
                    row.TransactionType,
                    row.TransactionDate,
                    row.DocumentType,
                    row.DocumentName,
                    row.Review,
                    request.ModifiedBy);
                await _repo.UpsertBatchPaymentAsync(rowRequest, cancellationToken);
            }

            var invoiceNumbers = draft.Rows
                .Select(x => x.InvoiceNumber.Trim().ToUpperInvariant())
                .Where(x => x.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            await _repo.ApplySaveAccountReceivableSideEffectsAsync(invoiceNumbers, committedBatchId, request.ModifiedBy, cancellationToken);
            await _repo.ClearDraftAsync(userKey, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var createdMessage = request.BatchId > 0
                ? $"{effectiveRequest.BatchNumber} - Batch created."
                : null;
            return new BatchOperationResponse(true, committedBatchId, effectiveRequest.BatchNumber, createdMessage);
        }

        // Legacy parity: when there are no staged rows, this endpoint updates/saves batch metadata only.
        var batchInfoId = await _repo.AccountsSaveAsync(effectiveRequest, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        if (batchInfoId == -1)
        {
            return new BatchOperationResponse(
                false,
                request.BatchId,
                effectiveRequest.BatchNumber,
                $"{effectiveRequest.BatchNumber} - Payments validation failed.");
        }

        var message = request.BatchId == 0
            ? $"{effectiveRequest.BatchNumber} - Batch created."
            : effectiveBatchStatus == 2
                ? $"{effectiveRequest.BatchNumber} - Batch posted."
                : $"{effectiveRequest.BatchNumber} - Batch updated.";
        return new BatchOperationResponse(true, batchInfoId, effectiveRequest.BatchNumber, message);
    }

    public async Task<int> UpsertBatchPaymentAsync(UpsertBatchPaymentRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy))
        {
            throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        }

        var id = await _repo.UpsertBatchPaymentAsync(request, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return id;
    }

    public async Task<bool> DeleteBatchPaymentAsync(int accountRcvId, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var deleted = await _repo.DeleteBatchPaymentAsync(accountRcvId, modifiedBy, cancellationToken);
        if (!deleted) return false;
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<decimal> GetTotalChargesAsync(string vendorInvoiceNumber, CancellationToken cancellationToken = default)
        => _repo.GetTotalChargesAsync(vendorInvoiceNumber, cancellationToken);

    public Task<IReadOnlyList<ProfitabilitySearchItemDto>> GetProfitabilityItemsAsync(ProfitabilitySearchRequest request, CancellationToken cancellationToken = default)
        => _repo.GetProfitabilityItemsAsync(request, cancellationToken);

    public Task<IReadOnlyList<ProfitabilitySearchItemDto>> GetVendorInvoiceNumbersAsync(string orderNumber, CancellationToken cancellationToken = default)
        => _repo.GetVendorInvoiceNumbersAsync(orderNumber, cancellationToken);

    public async Task<ProfitabilitySearchItemDto> UpsertProfitabilityAsync(UpsertProfitabilityRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy))
        {
            throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        }

        var result = await _repo.UpsertProfitabilityAsync(request, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return result;
    }

    public async Task<bool> DeleteProfitabilityAsync(DeleteProfitabilityRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.DeletedBy))
        {
            throw new ArgumentException("DeletedBy is required.", nameof(request.DeletedBy));
        }

        var deleted = await _repo.DeleteProfitabilityAsync(request, cancellationToken);
        if (!deleted) return false;
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<IReadOnlyList<TransactionTypeOptionDto>> GetTransactionTypesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TransactionTypeOptionDto>>(
        [
            new(1, "Check"),
            new(2, "ACH"),
            new(3, "Credit Card"),
            new(4, "Adjustment"),
            new(5, "Other")
        ]);

    public async Task<bool> UpsertAsync(int? id, string invoiceNumber, UpsertAccountReceivableRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ModifiedBy)) throw new ArgumentException("ModifiedBy is required.", nameof(request.ModifiedBy));
        if (string.IsNullOrWhiteSpace(invoiceNumber)) throw new ArgumentException("Invoice number is required.", nameof(invoiceNumber));

        var txType = AccountReceivableContractMapper.ToDomainTransactionType(request.TransactionType);
        var currentDue = AccountReceivablePostingPolicy.ComputeCurrentDue(request.AmountDue, request.AmountPaid, request.AdjustmentAmount);

        if (id is null)
        {
            await _repo.AddAsync(
                invoiceNumber.Trim(),
                request.AmountDue,
                request.AmountPaid,
                request.AdjustmentAmount,
                currentDue,
                request.CheckNumber,
                request.TransactionDate,
                txType,
                request.ModifiedBy,
                cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }

        var entry = await _repo.GetByIdAsync(id.Value, cancellationToken);
        if (entry is null) return false;

        var changed = entry.TryApplyPosting(
            request.AmountDue,
            request.AmountPaid,
            request.AdjustmentAmount,
            request.CheckNumber,
            request.TransactionDate,
            txType);
        if (!changed) return true;

        await _repo.UpdateAsync(
            entry.Id,
            entry.AmountDue,
            entry.AmountPaid,
            entry.AdjustmentAmount,
            entry.CurrentDue,
            entry.CheckNumber,
            entry.TransactionDate,
            entry.TransactionType,
            request.ModifiedBy,
            cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> VoidAsync(int id, string modifiedBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentException("ModifiedBy is required.", nameof(modifiedBy));

        var entry = await _repo.GetByIdAsync(id, cancellationToken);
        if (entry is null) return false;

        var changed = entry.TryVoid();
        if (!changed) return true;

        await _repo.VoidAsync(id, modifiedBy, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<AccountReceivableParityPreviewResponse> PreviewParityAsync(AccountReceivableParityPreviewRequest request, CancellationToken cancellationToken = default)
    {
        var status = AccountReceivableStatusPolicy.Resolve(request.IsDeleted, request.AmountDue, request.AmountPaid, request.AdjustmentAmount);
        var currentDue = AccountReceivablePostingPolicy.ComputeCurrentDue(request.AmountDue, request.AmountPaid, request.AdjustmentAmount);
        var noOpUpdate = string.Equals(request.ExistingCheckNumber?.Trim(), request.IncomingCheckNumber?.Trim(), StringComparison.OrdinalIgnoreCase);
        return Task.FromResult(new AccountReceivableParityPreviewResponse(
            AccountReceivableContractMapper.ToContractStatus(status),
            currentDue,
            request.DuplicateExists,
            noOpUpdate));
    }
}
