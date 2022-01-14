using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Json;

using DrrrAsync.Core;

namespace DrrrAsync.Helpers
{
    public static class HttpExtensions
    {
        static Logging.BasicLogger HttpLogger = new ("HTTP", Logging.LogLevel.All);

        public static async Task<string> PostAsync(this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> collection)
        {
            await RateLimiter.WaitPost();
            HttpResponseMessage result;
            try
            {
                result = await client.PostAsync(url, new FormUrlEncodedContent(collection));
            }
            catch ( Exception e )
            {
                HttpLogger.Error("HTTP Exception", e);
                return null;
            }
            RateLimiter.Release();
            return await result.Content.ReadAsStringAsync();
        }

        public static async Task<T> GetJsonAsync<T>(this HttpClient client, string url)
        {
            await RateLimiter.WaitGet();
            T result;
            try
            {
                result = await client.GetFromJsonAsync<T>(url);
            }
            catch ( Exception e )
            {
                HttpLogger.Error("HTTP Exception", e);
                return default;
            }
            RateLimiter.Release();
            return result;
        }
    }
}