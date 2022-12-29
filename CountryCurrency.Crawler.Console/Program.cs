using CountryCurrency.Crawler.Common;
using CountryCurrency.Crawler.Domain.Uol;
using HtmlAgilityPack;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CountryCurrency.Crawler.Console
{
    public static class WebUtils
    {
        public static Encoding GetEncodingFrom(
            NameValueCollection responseHeaders,
            Encoding defaultEncoding = null)
        {
            if (responseHeaders == null)
                throw new ArgumentNullException("responseHeaders");

            //Note that key lookup is case-insensitive
            var contentType = responseHeaders["Content-Type"];
            if (contentType == null)
                return defaultEncoding;

            var contentTypeParts = contentType.Split(';');
            if (contentTypeParts.Length <= 1)
                return defaultEncoding;

            var charsetPart =
                contentTypeParts.Skip(1).FirstOrDefault(
                    p => p.TrimStart().StartsWith("charset", StringComparison.InvariantCultureIgnoreCase));
            if (charsetPart == null)
                return defaultEncoding;

            var charsetPartParts = charsetPart.Split('=');
            if (charsetPartParts.Length != 2)
                return defaultEncoding;

            var charsetName = charsetPartParts[1].Trim();
            if (charsetName == "")
                return defaultEncoding;

            try
            {
                return Encoding.GetEncoding(charsetName);
            }
            catch (ArgumentException ex)
            {
                throw new Exception(
                    "The server returned data in an unknown encoding: " + charsetName,
                    ex);
            }
        }
    }
    public static class WebClientExtensions
    {
        public static string DownloadStringAwareOfEncoding(this WebClient webClient, Uri uri)
        {
            var rawData = webClient.DownloadData(uri);
            var encoding = WebUtils.GetEncodingFrom(webClient.ResponseHeaders, Encoding.UTF8);
            return encoding.GetString(rawData);
        }
    }
    class MyClient : WebClient
    {
        public bool HeadOnly { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest req = base.GetWebRequest(address);
            if (HeadOnly && req.Method == "GET")
            {
                req.Method = "HEAD";
            }
            return req;
        }
    }
    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }
    class Program
    {
        public static string DownloadString(WebClient webClient, String address, Encoding encoding)
        {
            byte[] buffer = webClient.DownloadData(address);

            byte[] bom = encoding.GetPreamble();

            if ((0 == bom.Length) || (buffer.Length < bom.Length))
            {
                return encoding.GetString(buffer);
            }

            for (int i = 0; i < bom.Length; i++)
            {
                if (buffer[i] != bom[i])
                {
                    return encoding.GetString(buffer);
                }
            }

            return encoding.GetString(buffer, bom.Length, buffer.Length - bom.Length);
        }
        static void Main(string[] args)
        {
            MyWebClient webClient = new MyWebClient();
            // call twice 
            // (or to just do a HEAD, see http://stackoverflow.com/questions/3268926/head-with-webclient)
            string t23 = webClient.DownloadString("https://futemax.gratis/assistir-fluminense-x-chapecoense-ao-vivo-online-hd-09122021/");
            var contentType = webClient.ResponseHeaders["Content-Type"];
            var charset = Regex.Match(contentType, "charset=([^;]+)").Groups[1].Value;

            webClient.Encoding = Encoding.GetEncoding(charset);
            var s = webClient.DownloadString("http://www.yahoo.com/");

            string sr = webClient.DownloadString("http://www.yahoo.com");

            String utf8 = DownloadString(webClient, "http://www.yahoo.com", Encoding.UTF8);

            string html = string.Empty;

            MyClient wc = new MyClient();
            wc.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            wc.Headers.Add("sec-ch-ua-mobile", "0");
            wc.Headers.Add("sec-ch-ua-platform", "Windows");
            wc.Headers.Add("sec-fetch-dest", "document");
            wc.Headers.Add("sec-fetch-mode", "navigate");
            wc.Headers.Add("sec-fetch-site", "cross-site");
            wc.Headers.Add("upgrade-insecure-requests", "1");

            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "");

            var stream = client.OpenRead("http://www.yahoo.com");
            //StreamReader sr = new StreamReader(stream);
            //string s2 = sr.ReadToEnd();

            //string s = wc.DownloadStringAwareOfEncoding(new Uri("http://futemax.gratis/"));

            byte[] body = wc.DownloadData("http://futemax.gratis/");

            //string s = Encoding.UTF8.GetString(body);

            string type = wc.ResponseHeaders["content-type"];

            wc.HeadOnly = false;

            if (type.StartsWith(@"text/"))
            {
                html = wc.DownloadString("http://futemax.gratis/");
            }

            //CookieWebClient wc = new CookieWebClient();
            //string html = wc.DownloadString("https://economia.uol.com.br/cotacoes/cambio/");

            Domain.Uol.CountryCurrency[] countriesCurrency = GetCountriesParse(html);

            string json = wc.DownloadString($"https://api.cotacoes.uol.com/currency/intraday/list/?format=JSON&fields=bidvalue,askvalue,maxbid,minbid,variationbid,variationpercentbid,openbidvalue,date&currency={countriesCurrency.FirstOrDefault().DataId}&");

            CurrencyData cData = JsonSerializer.Deserialize<CurrencyData>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            CurrencyDataDoc cDataDoc = cData.Docs.FirstOrDefault();
        }
        private static Domain.Uol.CountryCurrency[] GetCountriesParse(string html) 
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='financial-market-submenu']//ul[@class='target']");

            return node.SelectNodes("./li[@class='select-option']")
                       .Select(nd => new Domain.Uol.CountryCurrency()
                       {
                           DataId = nd.GetAttributeValue("data-id", string.Empty).ToInt16(),
                           DataUrl = nd.GetAttributeValue("data-url", string.Empty),
                           CountryCode = nd.SelectSingleNode("./*[@class='code']")?.InnerText,
                           CountryName = nd.SelectSingleNode("./*[@class='country-name']")?.InnerText,
                           CurrencyName = nd.SelectSingleNode("./*[@class='currency-name']")?.InnerText
                       })
                       .ToArray();
        }
    }
}
