using Microsoft.AspNetCore.Http;

namespace awisk.common.Interfaces
{
    public interface IApiService
    {
        Task<bool> DeleteAsync(Uri url, string? bearerToken = null, Dictionary<string, string>? headers = null);
        public Task<T?> GetAsync<T>(Uri url, string? bearerToken = null);
        public Task<TResponse?> PostAsync<TRequest, TResponse>(Uri url, TRequest data, string? bearerToken = null, Dictionary<string, string>? headers = null);
        Task<TResponse?> PutAsync<TRequest, TResponse>(Uri url, TRequest data, string? bearerToken = null, Dictionary<string, string>? headers = null) where TResponse : class;
        public Task<TResponse?> PostWithFileAsync<TRequest, TResponse>(Uri url, TRequest data, IFormFile file, string fileObjectName, string? bearerToken = null, Dictionary<string, string>? headers = null);
    }
}
