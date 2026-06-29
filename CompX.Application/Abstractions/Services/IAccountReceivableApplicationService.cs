using CompX.SharedKernel.Contracts.AccountReceivable.Dtos;
using CompX.SharedKernel.Contracts.AccountReceivable.Requests;
using CompX.SharedKernel.Contracts.AccountReceivable.Responses;

namespace CompX.Application.Abstractions.Services;

public interface IAccountReceivableApplicationService
{
    Task<AccountReceivableSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountReceivableLedgerItemDto>> GetByInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountReceivableInvoiceOptionDto>> SearchInvoicesAsync(string invoiceLike, CancellationToken cancellationToken = default);
    Task<AccountReceivableAmountDueDetailsDto> GetAmountDueDetailsByInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdjustmentCodeDto>> GetAdjustmentCodesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransactionTypeOptionDto>> GetTransactionTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountReceivableBatchLookupDto>> SearchBatchNumbersAsync(string searchValue, CancellationToken cancellationToken = default);
    Task<AccountReceivableBatchDetailsDto?> GetBatchDetailsAsync(string batchNumber, CancellationToken cancellationToken = default);
    Task<AccountReceivableBatchDetailsDto> GetBatchPaymentsAsync(int batchId, CancellationToken cancellationToken = default);
    Task<BatchOperationResponse> AccountsAsync(string userKey, SaveBatchRequest request, CancellationToken cancellationToken = default);
    Task<int> UpsertBatchPaymentAsync(UpsertBatchPaymentRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteBatchPaymentAsync(int accountRcvId, string modifiedBy, CancellationToken cancellationToken = default);
    Task<AccountReceivableDraftSnapshotDto> GetDraftAsync(string userKey, CancellationToken cancellationToken = default);
    Task<AccountReceivableDraftSnapshotDto> AddDraftGridRowAsync(string userKey, AddAccountReceivableGridRowRequest request, CancellationToken cancellationToken = default);
    Task<AccountReceivableDraftSnapshotDto> UpdateDraftGridRowAsync(string userKey, UpdateDraftRowRequest request, CancellationToken cancellationToken = default);
    Task<AccountReceivableDraftSnapshotDto> DeleteDraftGridRowAsync(string userKey, int rowId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalChargesAsync(string vendorInvoiceNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProfitabilitySearchItemDto>> GetProfitabilityItemsAsync(ProfitabilitySearchRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProfitabilitySearchItemDto>> GetVendorInvoiceNumbersAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<ProfitabilitySearchItemDto> UpsertProfitabilityAsync(UpsertProfitabilityRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProfitabilityAsync(DeleteProfitabilityRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpsertAsync(int? id, string invoiceNumber, UpsertAccountReceivableRequest request, CancellationToken cancellationToken = default);
    Task<bool> VoidAsync(int id, string modifiedBy, CancellationToken cancellationToken = default);
    Task<AccountReceivableParityPreviewResponse> PreviewParityAsync(AccountReceivableParityPreviewRequest request, CancellationToken cancellationToken = default);
}
