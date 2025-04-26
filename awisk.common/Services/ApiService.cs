using awisk.common.Interfaces;
using System.Net.Http.Headers;
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

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(Uri url, TRequest data, string? bearerToken = null)
        {
            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            using var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content).ConfigureAwait(true);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }
    }
}
