using System.Text;
using System.Text.Json;

namespace AgroScan.API.Utilities
{
    public class WebApiUtility : IWebApiUtility
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WebApiUtility> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public WebApiUtility(HttpClient httpClient, ILogger<WebApiUtility> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<T?> GetAsync<T>(string url, Dictionary<string, string>? headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddHeaders(request, headers);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GET request to {Url}", url);
                throw;
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data, Dictionary<string, string>? headers = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };
                AddHeaders(request, headers);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in POST request to {Url}", url);
                throw;
            }
        }

        public async Task<T?> PutAsync<T>(string url, object data, Dictionary<string, string>? headers = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = content
                };
                AddHeaders(request, headers);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PUT request to {Url}", url);
                throw;
            }
        }

        public async Task<T?> DeleteAsync<T>(string url, Dictionary<string, string>? headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, url);
                AddHeaders(request, headers);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DELETE request to {Url}", url);
                throw;
            }
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            try
            {
                return await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending HTTP request to {Url}", request.RequestUri);
                throw;
            }
        }

        public async Task<string> GetStringAsync(string url, Dictionary<string, string>? headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddHeaders(request, headers);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting string from {Url}", url);
                throw;
            }
        }

        public async Task<byte[]> GetByteArrayAsync(string url, Dictionary<string, string>? headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddHeaders(request, headers);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting byte array from {Url}", url);
                throw;
            }
        }

        private static void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}
