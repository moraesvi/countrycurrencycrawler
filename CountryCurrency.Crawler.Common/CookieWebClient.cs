using System;
using System.Net;

namespace CountryCurrency.Crawler.Common
{
    public class CookieWebClient : WebClient
    {
        public DateTime _session;
        public CookieWebClient() 
        {
            _session = DateTime.MinValue;
        }
        public CookieWebClient(CookieContainer cookieContainer)
        {
            CookieContainer = cookieContainer;
        }
        public DateTime Session => _session;
        public CookieContainer CookieContainer { get; } = new CookieContainer();
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest request = base.GetWebRequest(uri);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = CookieContainer;
            }

            _session = DateTime.UtcNow;

            return request;
        }
    }
}
