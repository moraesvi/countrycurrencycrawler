using CountryCurrency.Crawler.Common;
using CountryCurrency.Crawler.Domain.Uol;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace CountryCurrency.Crawler.Parse
{
    public class CountriesParse : ICountriesParse
    {
        private readonly HtmlDocument _htmlDocument;
        public CountriesParse(HtmlDocument htmlDocument) 
        {
            _htmlDocument = htmlDocument;
        }
        public Domain.Uol.CountryCurrency[] Parse(string html)
        {
            _htmlDocument.LoadHtml(html);

            HtmlNode node = _htmlDocument.DocumentNode.SelectSingleNode("//div[@class='financial-market-submenu']//ul[@class='target']");

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
