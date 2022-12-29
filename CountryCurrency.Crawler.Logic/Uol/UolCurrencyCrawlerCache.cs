using CountryCurrency.Crawler.Domain.Interface.Uol;
using CountryCurrency.Crawler.Domain.Uol;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace CountryCurrency.Crawler.Logic
{
    public class UolCurrencyCrawlerCache : IUolCurrencyCrawlerCache, IDisposable
    {
        private const int DAYS_CACHE_EXPIRATION = 5;
        private const int MINUTES_CACHE_EXPIRATION = 5;

        private readonly IMemoryCache _memoryCache;
        private readonly IUolCurrencyCrawler _uolCurrency;

        private MemoryCacheEntryOptions _cacheEntryOptionsGetCountries;
        private MemoryCacheEntryOptions _cacheEntryOptionsGetCountryCurrencyData;
        public UolCurrencyCrawlerCache(IMemoryCache memoryCache, IUolCurrencyCrawler uolCurrency)
        {
            _memoryCache = memoryCache;
            _uolCurrency = uolCurrency;
            _cacheEntryOptionsGetCountries = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(DAYS_CACHE_EXPIRATION))
                                                                          .RegisterPostEvictionCallback(PostEvictionCallback, _memoryCache);
            _cacheEntryOptionsGetCountryCurrencyData = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(MINUTES_CACHE_EXPIRATION))
                                                                                    .RegisterPostEvictionCallback(PostEvictionCallback, _memoryCache);
        }
        public async Task<Domain.Uol.CountryCurrency[]> GetCountries()
        {
            if (!_memoryCache.TryGetValue(nameof(GetCountries), out Domain.Uol.CountryCurrency[] cacheValue))
            {
                cacheValue = await _uolCurrency.GetCountries();

                _memoryCache.Set(nameof(GetCountries), cacheValue, _cacheEntryOptionsGetCountries);
            }

            return cacheValue;
        }
        public async Task<CountryCurrencyData> GetCountryCurrencyData(Domain.Uol.CountryCurrency countryCurrency)
        {
            if (!_memoryCache.TryGetValue(nameof(GetCountryCurrencyData), out CountryCurrencyData cacheValue))
            {
                cacheValue = await _uolCurrency.GetCountryCurrencyData(countryCurrency);

                _memoryCache.Set(nameof(GetCountryCurrencyData), cacheValue, _cacheEntryOptionsGetCountryCurrencyData);
            }

            return cacheValue;
        }
        public async Task<CountryCurrencyData[]> GetCountriesCurrencyData(Domain.Uol.CountryCurrency[] countriesCurrency)
        {
            if (!_memoryCache.TryGetValue(nameof(GetCountriesCurrencyData), out CountryCurrencyData[] cacheValue))
            {
                cacheValue = await _uolCurrency.GetCountriesCurrencyData(countriesCurrency);

                _memoryCache.Set(nameof(GetCountriesCurrencyData), cacheValue, _cacheEntryOptionsGetCountryCurrencyData);
            }

            return cacheValue;
        }
        public void Dispose()
        {
            _uolCurrency.Dispose();
        }
        private static void PostEvictionCallback(object cacheKey, object cacheValue, EvictionReason evictionReason, object state)
        {
            if (evictionReason == EvictionReason.Expired)
                ((IMemoryCache)state).Remove(cacheKey);
        }
    }
}
