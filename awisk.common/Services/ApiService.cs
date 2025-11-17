using awisk.common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;

namespace awisk.common.Services
{
    public class ApiService(HttpClient httpClient) : IApiService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<T?> GetAsync<T>(Uri url, string? bearerToken = null)
        {
            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var response = await _httpClient.GetAsync(url).ConfigureAwait(true);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(Uri url, TRequest data, string? bearerToken = null, Dictionary<string, string>? headers = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            using var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request).ConfigureAwait(true);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }


        public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        Uri url, TRequest data, string? bearerToken = null, Dictionary<string, string>? headers = null)
        where TResponse : class, new()
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(bearerToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return new();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }

        public async Task<bool> DeleteAsync(
        Uri url, string? bearerToken = null, Dictionary<string, string>? headers = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);

            if (!string.IsNullOrEmpty(bearerToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<TResponse?> PostWithFileAsync<TRequest, TResponse>(
                    Uri url,
                    TRequest data,
                    IFormFile file,
                    string fileObjectName,
                    string? bearerToken = null,
                    Dictionary<string, string>? headers = null)
        {
            if (!string.IsNullOrEmpty(bearerToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            if (headers != null)
            {
                foreach (var header in headers)
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            using var form = new MultipartFormDataContent();

            // Serialize the rest of the model and add it as string parts (one per property)
            var props = typeof(TRequest).GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(data);
                if (value != null)
                    form.Add(new StringContent(value.ToString()!), prop.Name);
            }

            // Add the file
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms).ConfigureAwait(false);
            ms.Position = 0;
            var fileContent = new StreamContent(ms);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); // This is key
            form.Add(fileContent, fileObjectName, file.FileName);

            var response = await _httpClient.PostAsync(url, form).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }

    }
}
