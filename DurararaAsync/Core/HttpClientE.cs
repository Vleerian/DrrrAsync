using System;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Collections.Generic;

namespace DrrrBot.Core
{
    /// <summary>
    /// A subclass of WebClient that saves cookies.
    /// </summary>
    public class HttpClientE : HttpClient
    {
        public CookieContainer m_container { get; private set; }
        private readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Post_String wraps UploadValuesTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to post to</param>
        /// <param name="Data">The Key/Value pairs you want to send</param>
        /// <returns>The raw text of the response</returns>
        public async Task<string> Post_String(string url, Dictionary<string, string> Data)
        {
            await Lock.WaitAsync();
            var Response = await PostAsync(url, new FormUrlEncodedContent(Data));
            Lock.Release();

            return await Response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Post_Json wraps UploadValuesTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to post to</param>
        /// <param name="Data">The Key/Value pairs you want to send</param>
        /// <returns>A JObject parsed from the response</returns>
        public async Task<JObject> Post_Json(string url, Dictionary<string, string> Data) => JObject.Parse(await Post_String(url, Data));

        /// <summary>
        /// Get_String wraps DownloadStringTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to get a string from</param>
        /// <returns>The raw text of the response</returns>
        public async Task<string> Get_String(string url)
        {
            await Lock.WaitAsync();
            string Response = await GetStringAsync(url);
            Lock.Release();

            return Response;
        }

        /// <summary>
        /// Get_Json wraps DownloadStringTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to get a string from</param>
        /// <returns>A JObject parsed from the response</returns>
        public async Task<JObject> Get_Json(string url) => JObject.Parse(await Get_String(url));
    }
}
