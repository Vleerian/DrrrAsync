using System;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Collections.Generic;

using DrrrAsyncBot.Helpers;

namespace DrrrAsyncBot.Core
{
    /// <summary>
    /// A subclass of WebClient that saves cookies.
    /// </summary>
    public class HttpClientE : HttpClient
    {
        public readonly CookieContainer m_container;
        public readonly WebProxy Proxy;

        public HttpClientE() : base() { }
        public HttpClientE(HttpMessageHandler handler) : base(handler) { }
        public HttpClientE(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler) { }

        public static HttpClientHandler GetProxyHandler(string ProxyURI)
        {
            var proxy = new WebProxy(ProxyURI, false);
            return new HttpClientHandler() {
                Proxy = proxy
            };
        }

        public static HttpClientHandler GetProxyHandler(string ProxyURI, int Port)
        {
            var proxy = new WebProxy(ProxyURI, Port);
            return new HttpClientHandler()
            {
                Proxy = proxy
            };
        }

        public static HttpClientE GetProxyClient(string ProxyURI) =>
            new HttpClientE(GetProxyHandler(ProxyURI), false);

        public static HttpClientE GetProxyClient(string ProxyURI, int Port) =>
            new HttpClientE(GetProxyHandler(ProxyURI, Port), false);

        /// <summary>
        /// Post_String wraps UploadValuesTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to post to</param>
        /// <param name="Data">The Key/Value pairs you want to send</param>
        /// <returns>The raw text of the response</returns>
        public async Task<string> Post_String(string url, Dictionary<string, string> Data)
        {
            await RateLimiter.WaitPost();
            Exception tmp = null;
            try {
                var response = await PostAsync(url, new FormUrlEncodedContent(Data));
                RateLimiter.Release();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e) { 
                tmp = e;
            }
            
            RateLimiter.Release();
            if(tmp != null)
                throw tmp;

            return "";
        }

        /// <summary>
        /// Post_Json wraps UploadValuesTaskAsync and JObject.Parse
        /// </summary>
        /// <param name="url">The URL you want to post to</param>
        /// <param name="Data">The Key/Value pairs you want to send</param>
        /// <returns>A JObject parsed from the response</returns>
        public async Task<JObject> Post_Json(string url, Dictionary<string, string> Data)
        {
            string raw = await Post_String(url, Data);
            if(raw != null && raw != "")
                return JObject.Parse(raw);
            return null;
        }

        /// <summary>
        /// Post_Object wraps UploadValuesTaskAsync and JsonConvert.SerializeObject
        /// </summary>
        /// <param name="url">The URL you want to post to</param>
        /// <param name="Data">The Key/Value pairs you want to send</param>
        /// <returns>The raw response data (string)</returns>
        public Task<string> Post_Object(string url, Dictionary<string, string> Data) =>
            Post_String(url, Data);

        /// <summary>
        /// Get_String wraps DownloadStringTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to get a string from</param>
        /// <returns>The raw text of the response</returns>
        public async Task<string> Get_String(string url)
        {
            await RateLimiter.WaitGet();
            Exception tmp = null;
            try {
                string Response = await GetStringAsync(url);
                RateLimiter.Release();
                return Response;
            }
            catch (Exception e) { 
                tmp = e;
            }
            
            RateLimiter.Release();
            if(tmp != null)
                throw tmp;

            return "";
        }

        /// <summary>
        /// Get_Json wraps DownloadStringTaskAsync and JObject.Parse
        /// </summary>
        /// <param name="url">The URL you want to get a string from</param>
        /// <returns>A JObject parsed from the response</returns>
        public async Task<JObject> Get_Json(string url)
        {
            string raw = await Get_String(url);
            if(raw != null && raw != "")
                return JObject.Parse(raw);
            return null;
        }

        /// <summary>
        /// Get_Object wraps DownloadStringTaskAsync and JsonConvert.DeserializeObject
        /// </summary>
        /// <param name="url">The URL you want to get a string from</param>
        /// <returns>A JObject parsed from the response</returns>
        public async Task<T> Get_Object<T>(string url)
        {
            string raw = await Get_String(url);
            if(raw != null && raw != "")
                return JsonConvert.DeserializeObject<T>(raw);
            return default;
        }
    }
}
