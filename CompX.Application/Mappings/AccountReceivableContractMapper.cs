using CompX.Domain.AccountReceivable;
using CompX.SharedKernel.Contracts.Common.Dtos;

namespace CompX.Application.Mappings;

public static class AccountReceivableContractMapper
{
    public static AccountReceivableTransactionType ToDomainTransactionType(AccountReceivableTransactionTypeContract type) => type switch
    {
        AccountReceivableTransactionTypeContract.Payment => AccountReceivableTransactionType.Payment,
        AccountReceivableTransactionTypeContract.Adjustment => AccountReceivableTransactionType.Adjustment,
        AccountReceivableTransactionTypeContract.Reversal => AccountReceivableTransactionType.Reversal,
        AccountReceivableTransactionTypeContract.PreviousCharges => AccountReceivableTransactionType.PreviousCharges,
        AccountReceivableTransactionTypeContract.ReverseCharges => AccountReceivableTransactionType.ReverseCharges,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported account receivable transaction type.")
    };

    public static AccountReceivableStatusContract ToContractStatus(AccountReceivableStatus status) => status switch
    {
        AccountReceivableStatus.Open => AccountReceivableStatusContract.Open,
        AccountReceivableStatus.PartiallyPaid => AccountReceivableStatusContract.PartiallyPaid,
        AccountReceivableStatus.Paid => AccountReceivableStatusContract.Paid,
        AccountReceivableStatus.Void => AccountReceivableStatusContract.Void,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported account receivable status.")
    };
}
