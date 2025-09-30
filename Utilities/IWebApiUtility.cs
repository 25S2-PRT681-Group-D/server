using System.Text.Json;

namespace AgroScan.API.Utilities
{
    public interface IWebApiUtility
    {
        Task<T?> GetAsync<T>(string url, Dictionary<string, string>? headers = null);
        Task<T?> PostAsync<T>(string url, object data, Dictionary<string, string>? headers = null);
        Task<T?> PutAsync<T>(string url, object data, Dictionary<string, string>? headers = null);
        Task<T?> DeleteAsync<T>(string url, Dictionary<string, string>? headers = null);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        Task<string> GetStringAsync(string url, Dictionary<string, string>? headers = null);
        Task<byte[]> GetByteArrayAsync(string url, Dictionary<string, string>? headers = null);
    }
}
