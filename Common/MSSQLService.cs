using Dapper;
using Microsoft.Data.SqlClient;

namespace Common;

public class MSSQLService : IDBService, IDisposable
{
    private readonly SqlConnection _connection;
    private SqlTransaction? _transaction;

    public MSSQLService(string strConnection)
    {
        _connection = new SqlConnection(strConnection);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string strSql, object? Param = null)
    {
        return await _connection.QueryAsync<T>(strSql, Param, _transaction);
    }

    public async Task<int> ExecuteAsync(string strSql, object? Param = null)
    {
        return await _connection.ExecuteAsync(strSql, Param, _transaction);
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await Task.Run(() => _transaction.Commit()); 
            _transaction = _connection.BeginTransaction();
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await Task.Run(() => _transaction.Rollback()); 
            _transaction = _connection.BeginTransaction();
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection.Dispose();
    }
}
