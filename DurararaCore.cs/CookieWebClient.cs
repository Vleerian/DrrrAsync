using System;
using System.Net;

namespace DrrrAsync
{
    public class CookieWebClient : WebClient
    {
        public CookieContainer m_container { get; private set; }

        public CookieWebClient() : base() =>
            m_container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest webRequest)
                webRequest.CookieContainer = m_container;

            return request;
        }
    }
}
