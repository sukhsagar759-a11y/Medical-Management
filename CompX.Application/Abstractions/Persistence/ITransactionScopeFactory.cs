namespace CompX.Application.Abstractions.Persistence;

public interface ITransactionScopeFactory
{
    Task<IAppTransaction> BeginAsync(CancellationToken cancellationToken = default);
}

public interface IAppTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
