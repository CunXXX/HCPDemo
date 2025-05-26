namespace Common;

public interface IDBService
{
    Task<IEnumerable<T>> QueryAsync<T>(string strSql, object? Param = null);
    Task<int> ExecuteAsync(string strSql, object? Param = null);
    Task CommitAsync();
    Task RollbackAsync();
}
