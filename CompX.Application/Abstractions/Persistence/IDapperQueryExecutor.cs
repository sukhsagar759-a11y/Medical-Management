using System.Data;

namespace CompX.Application.Abstractions.Persistence;

public interface IReadDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public interface IDapperQueryExecutor
{
    Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default);
}
