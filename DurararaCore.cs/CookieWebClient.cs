using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace DrrrAsync
{
    /// <summary>
    /// A subclass of WebClient that saves cookies.
    /// </summary>
    public class CookieWebClient : WebClient
    {
        public CookieContainer m_container { get; private set; }
        private readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        // TODO: Wrap DownloadStringTaskAsync to improve readability of client code.
        // TODO: Wrap UploadValuesTaskAsync to improve readability of client code.
        /*
        string GetString(string url)
        JObject GetJson(string url)
        
        string Post(string url, NameValueCollection Data)
        JObject Post(string url, NameValueCollection Data)
        */

        /// <summary>
        /// Post_String wraps UploadValuesTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to post to</param>
        /// <param name="Data">The Key/Value pairs you want to send</param>
        /// <returns>The raw text of the response</returns>
        public async Task<string> Post_String(string url, NameValueCollection Data)
        {
            await Lock.WaitAsync();
            byte[] Response = await UploadValuesTaskAsync(new Uri(url), "POST", Data);
            Lock.Release();

            return Encoding.ASCII.GetString(Response);
        }

        /// <summary>
        /// Post_Json wraps UploadValuesTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to post to</param>
        /// <param name="Data">The Key/Value pairs you want to send</param>
        /// <returns>A JObject parsed from the response</returns>
        public async Task<JObject> Post_Json(string url, NameValueCollection Data) => JObject.Parse(await Post_String(url, Data));

        /// <summary>
        /// Get_String wraps DownloadStringTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to get a string from</param>
        /// <returns>The raw text of the response</returns>
        public async Task<string> Get_String(string url)
        {
            await Lock.WaitAsync();
            string Response = await DownloadStringTaskAsync(new Uri(url));
            Lock.Release();

            return Response;
        }

        /// <summary>
        /// Get_Json wraps DownloadStringTaskAsync to make it look nice in source
        /// </summary>
        /// <param name="url">The URL you want to get a string from</param>
        /// <returns>A JObject parsed from the response</returns>
        public async Task<JObject> Get_Json(string url) => JObject.Parse(await Get_String(url));

        /// <summary>
        /// CookieWebClient constructor. Instantiates an empty CookieContainer.
        /// </summary>
        public CookieWebClient() : base() =>
            m_container = new CookieContainer();

        /// <summary>
        /// An override that creates a webrequest using the stored cookie container.
        /// </summary>
        /// <param name="address">The address of the resource you're trying to access</param>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest webRequest)
                webRequest.CookieContainer = m_container;

            return request;
        }
    }
}
