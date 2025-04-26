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

        public T GetById<T, ID>(ID id) where T : class
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.Get<T>(id);
        }

        public T Insert<T>(T item) where T : class
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Insert(item);
            return item;
        }

        public void Insert<T>(IEnumerable<T> items) where T : class
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Insert(items);
        }

        public bool Update<T>(T item) where T : class
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            return connection.Update(item);
        }

        public bool Delete<T, ID>(ID id) where T : class
        {
            var entity = GetById<T, ID>(id);
            return Delete(entity);
        }

        public bool Delete<T>(T item) where T : class
        {
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
            if (page.HasValue)
            {
                int offset = page.Value;
                return $" OFFSET {offset} LIMIT {size}";
            }
            return string.Empty;
        }

        public static IEnumerable<IEnumerable<T>> CreateBatches<T>(IEnumerable<T> items)
        {
            var batches = new List<List<T>>();
            var tempItems = items.ToList();

            while (tempItems.Count != 0)
            {
                var batch = tempItems.Take(2000).ToList();
                batches.Add(batch);
                tempItems.RemoveRange(0, batch.Count);
            }
            return batches;
        }
    }
}
