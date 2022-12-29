using CountryCurrency.Crawler.Domain.Interface.Uol;
using CountryCurrency.Crawler.Domain.Uol;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CountryCurrency.Crawler.Logic
{
    public class UolCurrencyCrawlerBuilder : IUolCurrencyCrawlerBuilder
    {
        private readonly IUolCurrencyCrawler _currencyCrawler;
        private readonly IUolCurrencyCrawlerCache _currencyCrawlerCache;
        public UolCurrencyCrawlerBuilder(IUolCurrencyCrawler uolCurrencyCrawler, IUolCurrencyCrawlerCache currencyCrawlerCache)
        {
            _currencyCrawler = uolCurrencyCrawler;
            _currencyCrawlerCache = currencyCrawlerCache;
        }
        public bool CoutriesCurrencyMaxToReturnIsValid(string[] dataIds) => _currencyCrawler.CoutriesCurrencyMaxToReturnIsValid(dataIds);
        public async Task<Domain.Uol.CountryCurrency[]> GetCountries()
        {
            return await _currencyCrawlerCache.GetCountries();
        }
        public async Task<CountryCurrencyData> BuildCountryCurrency(short dataId)
        {
            Domain.Uol.CountryCurrency[] countries = await _currencyCrawler.GetCountries();

            Domain.Uol.CountryCurrency country = countries.Where(c => c.DataId == dataId)
                                                          .FirstOrDefault();

            return await _currencyCrawlerCache.GetCountryCurrencyData(country);
        }
        public async Task<CountryCurrencyData[]> BuildCountriesCurrency(string[] dataIds)
        {
            Domain.Uol.CountryCurrency[] countries = await GetCountries();

            Domain.Uol.CountryCurrency[] countriesCurrency = countries.Where(c => dataIds.Contains(c.DataUrl))
                                                                      .ToArray() ?? new Domain.Uol.CountryCurrency[] { };

            return await _currencyCrawlerCache.GetCountriesCurrencyData(countriesCurrency);
        }
    }
}
