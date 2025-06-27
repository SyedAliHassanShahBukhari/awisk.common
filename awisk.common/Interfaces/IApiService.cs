using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.Interfaces
{
    public interface IApiService
    {
        public Task<T?> GetAsync<T>(Uri url, string? bearerToken = null, Dictionary<string, string>? headers = null);
        public Task<TResponse?> PostAsync<TRequest, TResponse>(Uri url, TRequest data, string? bearerToken = null, Dictionary<string, string>? headers = null);
        public Task<TResponse?> PostWithFileAsync<TRequest, TResponse>(Uri url, TRequest data, IFormFile file, string fileObjectName, string? bearerToken = null, Dictionary<string, string>? headers = null);
    }
}
