using awisk.common.Data.Db.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using System.Data;

namespace awisk.common.Data.Db
{
    public class RepositoryBasePostgreSql(string connectionString) : IRepositoryBase
    {
        public string ConnectionString { get; protected set; } = connectionString;

        public IEnumerable<T> GetAll<T>() where T : class
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.GetAll<T>();
        }

        /// <summary>
        /// Gets an entity by its ID. Returns null if not found.
        /// </summary>
        public T? GetById<T, ID>(ID id) where T : class
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.Get<T>(id);
        }

        public T Insert<T>(T item) where T : class
        {
            ArgumentNullException.ThrowIfNull(item);

            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Insert(item);
            return item;
        }

        public void Insert<T>(IEnumerable<T> items) where T : class
        {
            ArgumentNullException.ThrowIfNull(items);

            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Insert(items);
        }

        public bool Update<T>(T item) where T : class
        {
            ArgumentNullException.ThrowIfNull(item);

            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.Update(item);
        }

        public bool Delete<T, ID>(ID id) where T : class
        {
            var entity = GetById<T, ID>(id);
            if (entity == null)
            {
                return false;
            }

            return Delete(entity);
        }

        public bool Delete<T>(T item) where T : class
        {
            ArgumentNullException.ThrowIfNull(item);

            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.Delete(item);
        }

        public int Execute(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.Execute(command);
        }

        protected static int Execute(string connectionString, string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(connectionString);
            return connection.Execute(command);
        }

        public IReadOnlyList<dynamic> Query(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.Query(sql, parameters, commandType: commandType).AsList();
        }

        public IReadOnlyList<T> Query<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.Query<T>(command).AsList();
        }

        public dynamic QueryFirst(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QueryFirst(sql, parameters, commandType: commandType);
        }

        public T QueryFirst<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QueryFirst<T>(command);
        }

        public dynamic QueryFirstOrDefault(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QueryFirstOrDefault(sql, parameters, commandType: commandType);
        }

        public T QueryFirstOrDefault<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QueryFirstOrDefault<T>(command);
        }

        public async Task<int> DeleteByIdAsync(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.ExecuteAsync(sql, parameters, commandType: commandType).ConfigureAwait(false);
        }

        public dynamic QuerySingle(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QuerySingle(sql, parameters, commandType: commandType);
        }

        public T QuerySingle<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QuerySingle<T>(command);
        }

        public dynamic QuerySingleOrDefault(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QuerySingleOrDefault(sql, parameters, commandType: commandType);
        }

        public T QuerySingleOrDefault<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QuerySingleOrDefault<T>(command);
        }

        public static string GetPagingStatement(int? page, int? pageSize)
        {
            int size = pageSize ?? 100;
            if (page.HasValue && page.Value > 0)
            {
                int offset = (page.Value - 1) * size;
                return $" LIMIT {size} OFFSET {offset}";
            }
            return $" LIMIT {size}";
        }

        private const int DefaultBatchSize = 2000;

        /// <summary>
        /// Splits a collection into batches of a specified size for batch processing.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="items">The collection to batch</param>
        /// <param name="batchSize">The size of each batch (default: 2000)</param>
        /// <returns>An enumerable of batches</returns>
        public static IEnumerable<IEnumerable<T>> CreateBatches<T>(IEnumerable<T> items, int batchSize = DefaultBatchSize)
        {
            ArgumentNullException.ThrowIfNull(items);

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero.");
            }

            var tempItems = items.ToList();
            var batches = new List<List<T>>();

            while (tempItems.Count > 0)
            {
                var batch = tempItems.Take(batchSize).ToList();
                batches.Add(batch);
                tempItems.RemoveRange(0, batch.Count);
            }

            return batches;
        }

        // ───────────────────────────────────────────────
        // Async CRUD Operations
        // ───────────────────────────────────────────────

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.GetAllAsync<T>().ConfigureAwait(false);
        }

        public async Task<T?> GetByIdAsync<T, ID>(ID id) where T : class
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.GetAsync<T>(id).ConfigureAwait(false);
        }

        public async Task<T> InsertAsync<T>(T item) where T : class
        {
            ArgumentNullException.ThrowIfNull(item);

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.InsertAsync(item).ConfigureAwait(false);
            return item;
        }

        public async Task InsertAsync<T>(IEnumerable<T> items) where T : class
        {
            ArgumentNullException.ThrowIfNull(items);

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.InsertAsync(items).ConfigureAwait(false);
        }

        public async Task<bool> UpdateAsync<T>(T item) where T : class
        {
            ArgumentNullException.ThrowIfNull(item);

            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.UpdateAsync(item).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync<T, ID>(ID id) where T : class
        {
            T? byId = await GetByIdAsync<T, ID>(id).ConfigureAwait(false);
            if (byId == null)
            {
                return false;
            }

            return await DeleteAsync(byId).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync<T>(T item) where T : class
        {
            ArgumentNullException.ThrowIfNull(item);

            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.DeleteAsync(item).ConfigureAwait(false);
        }

        // ───────────────────────────────────────────────
        // Async Query Operations
        // ───────────────────────────────────────────────

        public async Task<int> ExecuteAsync(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.ExecuteAsync(command).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<dynamic>> QueryAsync(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            var result = await connection.QueryAsync(sql, parameters, commandType: commandType).ConfigureAwait(false);
            return result.AsList();
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            var result = await connection.QueryAsync<T>(command).ConfigureAwait(false);
            return result.AsList();
        }

        public async Task<dynamic> QueryFirstAsync(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QueryFirstAsync(sql, parameters, commandType: commandType).ConfigureAwait(false);
        }

        public async Task<T> QueryFirstAsync<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QueryFirstAsync<T>(command).ConfigureAwait(false);
        }

        public async Task<dynamic> QueryFirstOrDefaultAsync(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QueryFirstOrDefaultAsync(sql, parameters, commandType: commandType).ConfigureAwait(false);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(command).ConfigureAwait(false);
        }

        public async Task<dynamic> QuerySingleAsync(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QuerySingleAsync(sql, parameters, commandType: commandType).ConfigureAwait(false);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QuerySingleAsync<T>(command).ConfigureAwait(false);
        }

        public async Task<dynamic> QuerySingleOrDefaultAsync(string sql, object? parameters, CommandType commandType)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QuerySingleOrDefaultAsync(sql, parameters, commandType: commandType).ConfigureAwait(false);
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? parameters, CommandType commandType)
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QuerySingleOrDefaultAsync<T>(command).ConfigureAwait(false);
        }

        // ───────────────────────────────────────────────
        // Count Operations
        // ───────────────────────────────────────────────

        public int Count<T>() where T : class
        {
            var tableName = typeof(T).Name;
            var sql = $"SELECT COUNT(*) FROM \"{tableName}\"";
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QuerySingle<int>(sql);
        }

        public async Task<int> CountAsync<T>() where T : class
        {
            var tableName = typeof(T).Name;
            var sql = $"SELECT COUNT(*) FROM \"{tableName}\"";
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QuerySingleAsync<int>(sql).ConfigureAwait(false);
        }

        public int Count<T>(string sql, object? parameters, CommandType commandType) where T : class
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.QuerySingle<int>(command);
        }

        public async Task<int> CountAsync<T>(string sql, object? parameters, CommandType commandType) where T : class
        {
            var command = new CommandDefinition(sql, parameters, commandType: commandType);
            using var connection = new NpgsqlConnection(ConnectionString);
            return await connection.QuerySingleAsync<int>(command).ConfigureAwait(false);
        }

        // ───────────────────────────────────────────────
        // Exists Operations
        // ───────────────────────────────────────────────

        public bool Exists<T, ID>(ID id) where T : class
        {
            T? entity = GetById<T, ID>(id);
            return entity != null;
        }

        public async Task<bool> ExistsAsync<T, ID>(ID id) where T : class
        {
            T? entity = await GetByIdAsync<T, ID>(id).ConfigureAwait(false);
            return entity != null;
        }
    }
}
