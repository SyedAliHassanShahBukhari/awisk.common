using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.Data.Db.Interfaces
{
    public interface IRepositoryBase
    {
        IEnumerable<T> GetAll<T>() where T : class;
        T GetById<T, ID>(ID id) where T : class;
        T Insert<T>(T item) where T : class;
        void Insert<T>(IEnumerable<T> items) where T : class;
        bool Update<T>(T item) where T : class;
        bool Delete<T, ID>(ID id) where T : class;
        bool Delete<T>(T item) where T : class;

        int Execute(string sql, object? parameters, CommandType commandType);
        IReadOnlyList<dynamic> Query(string sql, object? parameters, CommandType commandType);
        IReadOnlyList<T> Query<T>(string sql, object? parameters, CommandType commandType);
        dynamic QueryFirst(string sql, object? parameters, CommandType commandType);
        T QueryFirst<T>(string sql, object? parameters, CommandType commandType);
        dynamic QueryFirstOrDefault(string sql, object? parameters, CommandType commandType);
        T QueryFirstOrDefault<T>(string sql, object? parameters, CommandType commandType);
        Task<int> DeleteByIdAsync(string sql, object? parameters, CommandType commandType);
        dynamic QuerySingle(string sql, object? parameters, CommandType commandType);
        T QuerySingle<T>(string sql, object? parameters, CommandType commandType);
        dynamic QuerySingleOrDefault(string sql, object? parameters, CommandType commandType);
        T QuerySingleOrDefault<T>(string sql, object? parameters, CommandType commandType);
    }
}
