using System;
using System.Net;

namespace DrrrAsync
{
    /// <summary>
    /// A subclass of WebClient that saves cookies.
    /// </summary>
    public class CookieWebClient : WebClient
    {
        public CookieContainer m_container { get; private set; }

        // TODO: Wrap DownloadStringTaskAsync to improve readability of client code.
        // TODO: Wrap UploadValuesTaskAsync to improve readability of client code.
        /*
        string GetString(string url)
        JObject GetJson(string url)
        
        string Post(string url, NameValueCollection Data)
        JObject Post(string url, NameValueCollection Data)
        */

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
