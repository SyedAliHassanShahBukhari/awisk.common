using awisk.common.Data.Db.Interfaces;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System.Data;

namespace awisk.common.Data.Db
{
    public class RepositoryBaseMySql(string connectionString) : IRepositoryBase
    {
        public string ConnectionString { get; protected set; } = connectionString;

        public IEnumerable<T> GetAll<T>() where T : class
        {
            using MySqlConnection connection = new(ConnectionString);
            return connection.GetAll<T>();
        }

        public T GetById<T, ID>(ID id) where T : class
        {
            using MySqlConnection connection = new(ConnectionString);
            return connection.Get<T>(id);
        }

        public T Insert<T>(T item) where T : class
        {
            using MySqlConnection connection = new(ConnectionString);
            connection.Insert(item);
            return item;
        }

        public void Insert<T>(IEnumerable<T> items) where T : class
        {
            using MySqlConnection connection = new(ConnectionString);
            connection.Insert(items);
        }

        public bool Update<T>(T item) where T : class
        {
            using MySqlConnection connection = new(ConnectionString);
            return connection.Update(item);
        }

        public bool Delete<T, ID>(ID id) where T : class
        {
            T byId = GetById<T, ID>(id);
            return Delete(byId);
        }

        public bool Delete<T>(T item) where T : class
        {
            using MySqlConnection connection = new(ConnectionString);
            return connection.Delete(item);
        }

        public int Execute(string commandText, object? parameters, CommandType commandType)
        {
            CommandType? commandType2 = commandType;
            CommandDefinition command = new(commandText, parameters, null, null, commandType2);
            using MySqlConnection cnn = new(ConnectionString);
            return cnn.Execute(command);
        }

        protected static int Execute(string connectionString, string commandText, object? parameters, CommandType commandType)
        {
            CommandType? commandType2 = commandType;
            CommandDefinition command = new(commandText, parameters, null, null, commandType2);
            using MySqlConnection cnn = new(connectionString);
            return cnn.Execute(command);
        }

        public IReadOnlyList<dynamic> Query(string sql, object? parameters, CommandType commandType)
        {
            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.Query(sql, parameters, commandType: commandType).AsList();
        }

        public IReadOnlyList<T> Query<T>(string sql, object? parameters, CommandType commandType)
        {
            CommandDefinition commandDefinition = new(sql, parameters, commandType: commandType);

            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.Query<T>(commandDefinition).AsList();
        }

        public dynamic QueryFirst(string sql, object? parameters, CommandType commandType)
        {
            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.QueryFirst(sql, parameters, commandType: commandType);
        }

        public T QueryFirst<T>(string commandText, object? parameters, CommandType commandType)
        {
            CommandDefinition commandDefinition = new(commandText, parameters, commandType: commandType);

            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.QueryFirst<T>(commandDefinition);
        }

        public dynamic QueryFirstOrDefault(string sql, object? parameters, CommandType commandType)
        {
            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.QueryFirstOrDefault(sql, parameters, commandType: commandType);
        }

        public T QueryFirstOrDefault<T>(string commandText, object? parameters, CommandType commandType)
        {
            CommandDefinition commandDefinition = new(commandText, parameters, commandType: commandType);

            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.QueryFirstOrDefault<T>(commandDefinition);
        }

        public async Task<int> DeleteByIdAsync(string sql, object? parameters, CommandType commandType)
        {
            using MySqlConnection sqlConnection = new(ConnectionString);
            return await sqlConnection.ExecuteAsync(sql, parameters, commandType: commandType).ConfigureAwait(false);
        }

        public dynamic QuerySingle(string sql, object? parameters, CommandType commandType)
        {
            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.QuerySingle(sql, parameters, commandType: commandType);
        }

        public T QuerySingle<T>(string commandText, object? parameters, CommandType commandType)
        {
            CommandDefinition commandDefinition = new(commandText, parameters, commandType: commandType);

            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.QuerySingle<T>(commandDefinition);
        }

        public dynamic QuerySingleOrDefault(string sql, object? parameters, CommandType commandType)
        {
            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.QuerySingleOrDefault(sql, parameters, commandType: commandType);
        }

        public T QuerySingleOrDefault<T>(string commandText, object? parameters, CommandType commandType)
        {
            CommandDefinition commandDefinition = new(commandText, parameters, commandType: commandType);

            using MySqlConnection sqlConnection = new(ConnectionString);
            return sqlConnection.QuerySingleOrDefault<T>(commandDefinition);
        }

        public static string GetPagingStatement(int? page, int? pageSize)
        {
            int num = 100;
            int num2;
            string result = "";
            if (pageSize.HasValue)
            {
                num = pageSize.Value;
            }

            if (page.HasValue)
            {
                num2 = page.Value;
                result = $" OFFSET {num2} ROWS FETCH NEXT {num} ROWS ONLY";
            }

            return result;
        }

        public static IEnumerable<IEnumerable<T>> CreateBatches<T>(IEnumerable<T> items)
        {
            List<List<T>> batches = [];
            List<T> batch;
            List<T> tempItems = [];
            tempItems.AddRange(items);

            while (tempItems.Count > 0)
            {
                batch = [.. tempItems.Take(2000)];
                batches.Add(batch);
                tempItems.RemoveRange(0, batch.Count);
            }

            return batches;
        }
    }
}
