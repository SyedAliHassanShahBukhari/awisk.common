using System.Data;

namespace awisk.common.Data.Db.Interfaces
{
    public interface IRepositoryBase
    {
        // Synchronous CRUD Operations
        IEnumerable<T> GetAll<T>() where T : class;
        T? GetById<T, ID>(ID id) where T : class;
        T Insert<T>(T item) where T : class;
        void Insert<T>(IEnumerable<T> items) where T : class;
        bool Update<T>(T item) where T : class;
        bool Delete<T, ID>(ID id) where T : class;
        bool Delete<T>(T item) where T : class;

        // Synchronous Query Operations
        int Execute(string sql, object? parameters, CommandType commandType);
        IReadOnlyList<dynamic> Query(string sql, object? parameters, CommandType commandType);
        IReadOnlyList<T> Query<T>(string sql, object? parameters, CommandType commandType);
        dynamic QueryFirst(string sql, object? parameters, CommandType commandType);
        T QueryFirst<T>(string sql, object? parameters, CommandType commandType);
        dynamic QueryFirstOrDefault(string sql, object? parameters, CommandType commandType);
        T QueryFirstOrDefault<T>(string sql, object? parameters, CommandType commandType);
        dynamic QuerySingle(string sql, object? parameters, CommandType commandType);
        T QuerySingle<T>(string sql, object? parameters, CommandType commandType);
        dynamic QuerySingleOrDefault(string sql, object? parameters, CommandType commandType);
        T QuerySingleOrDefault<T>(string sql, object? parameters, CommandType commandType);

        // Async CRUD Operations
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task<T?> GetByIdAsync<T, ID>(ID id) where T : class;
        Task<T> InsertAsync<T>(T item) where T : class;
        Task InsertAsync<T>(IEnumerable<T> items) where T : class;
        Task<bool> UpdateAsync<T>(T item) where T : class;
        Task<bool> DeleteAsync<T, ID>(ID id) where T : class;
        Task<bool> DeleteAsync<T>(T item) where T : class;

        // Async Query Operations
        Task<int> ExecuteAsync(string sql, object? parameters, CommandType commandType);
        Task<IReadOnlyList<dynamic>> QueryAsync(string sql, object? parameters, CommandType commandType);
        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? parameters, CommandType commandType);
        Task<dynamic> QueryFirstAsync(string sql, object? parameters, CommandType commandType);
        Task<T> QueryFirstAsync<T>(string sql, object? parameters, CommandType commandType);
        Task<dynamic> QueryFirstOrDefaultAsync(string sql, object? parameters, CommandType commandType);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? parameters, CommandType commandType);
        Task<dynamic> QuerySingleAsync(string sql, object? parameters, CommandType commandType);
        Task<T> QuerySingleAsync<T>(string sql, object? parameters, CommandType commandType);
        Task<dynamic> QuerySingleOrDefaultAsync(string sql, object? parameters, CommandType commandType);
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? parameters, CommandType commandType);
        Task<int> DeleteByIdAsync(string sql, object? parameters, CommandType commandType);

        // Count and Exists Operations
        int Count<T>() where T : class;
        Task<int> CountAsync<T>() where T : class;
        int Count<T>(string sql, object? parameters, CommandType commandType) where T : class;
        Task<int> CountAsync<T>(string sql, object? parameters, CommandType commandType) where T : class;
        bool Exists<T, ID>(ID id) where T : class;
        Task<bool> ExistsAsync<T, ID>(ID id) where T : class;

        // Transaction Support
        Task<T> ExecuteInTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> work);
        Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> work);
    }
}
