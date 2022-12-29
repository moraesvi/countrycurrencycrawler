using CountryCurrency.Crawler.Common;
using CountryCurrency.Crawler.Domain.Interface.Uol;
using CountryCurrency.Crawler.Domain.Uol;
using CountryCurrency.Crawler.Parse;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CountryCurrency.Crawler.Logic
{
    public class UolCurrencyCrawler : IUolCurrencyCrawler
    {
        private short UOL_COUTRIES_CURRENCY_MAX_TO_RETURN_ON_SINGLE_REQUEST = 5;
        private short UOL_CURRENCY_MINUTES_SESSION_TIME = 15;

        private readonly IConfiguration _config;
        private readonly ICountriesParse _countriesParse;
        private readonly CookieWebClient _cookieWebClient;

        private string _htmlSession;

        private static readonly int _maxRetryAttempts = 3;
        private static readonly TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(2);
        private static readonly AsyncRetryPolicy _retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(_maxRetryAttempts, i => _pauseBetweenFailures);
        public UolCurrencyCrawler(IConfiguration config, ICountriesParse countriesParse)
        {
            _config = config;
            _countriesParse = countriesParse;
            _cookieWebClient = new CookieWebClient();
        }
        public bool CoutriesCurrencyMaxToReturnIsValid(string[] dataIds) => dataIds != null && dataIds.Count() <= UOL_COUTRIES_CURRENCY_MAX_TO_RETURN_ON_SINGLE_REQUEST
                                                                                                               ? true : false;
        public bool CoutriesCurrencyMaxToReturnIsValid(Domain.Uol.CountryCurrency[] dataIds) => dataIds != null && dataIds.Count() <= UOL_COUTRIES_CURRENCY_MAX_TO_RETURN_ON_SINGLE_REQUEST
                                                                                                                                   ? true : false;
        public async Task<Domain.Uol.CountryCurrency[]> GetCountries()
        {
            await OpenSession();

            return _countriesParse.Parse(_htmlSession);
        }
        public async Task<CountryCurrencyData> GetCountryCurrencyData(Domain.Uol.CountryCurrency countryCurrency)
        {
            if (countryCurrency == null)
                return null;

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                await OpenSession();

                using (CookieWebClient wc = new CookieWebClient(_cookieWebClient.CookieContainer))//required for multithread
                {
                    string url = string.Concat(_config.GetSection("UolConfigUrl:CurrencyDataUrl").Value, $"&currency={countryCurrency.DataId}&");

                    string json = await wc.DownloadStringTaskAsync(url);

                    CurrencyData cData = JsonSerializer.Deserialize<CurrencyData>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    return new CountryCurrencyData(countryCurrency, cData.Docs.FirstOrDefault());
                }
            });
        }
        public async Task<CountryCurrencyData[]> GetCountriesCurrencyData(Domain.Uol.CountryCurrency[] countriesCurrency)
        {
            if (countriesCurrency == null)
                return new CountryCurrencyData[] { };

            if (!CoutriesCurrencyMaxToReturnIsValid(countriesCurrency))
                throw new InvalidOperationException("Ultrapassou o limite de países no request");

            List<Task> lstTaskCurrencyDataDoc = new List<Task>();
            List<CountryCurrencyData> lstCountryCurrencyData = new List<CountryCurrencyData>();

            foreach (Domain.Uol.CountryCurrency country in countriesCurrency)
            {
                Task tskCurrency = Task.Run(async () =>
                {
                    lstCountryCurrencyData.Add(await GetCountryCurrencyData(country));
                });

                lstTaskCurrencyDataDoc.Add(tskCurrency);
            }

            await Task.WhenAll(lstTaskCurrencyDataDoc.ToArray());

            return lstCountryCurrencyData.ToArray();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _cookieWebClient?.Dispose();
        }

        #region Private Methods
        private async Task<string> OpenSession()
        {
            if (SessionExpirationIsValid())
                return _htmlSession;

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                _htmlSession = await _cookieWebClient.DownloadStringTaskAsync(_config.GetSection("UolConfigUrl:CurrencyUrl")?.Value);
                return _htmlSession;
            });
        }
        private bool SessionExpirationIsValid()
        {
            if (_cookieWebClient.Session == DateTime.MinValue)
                return false;

            return (DateTime.UtcNow - _cookieWebClient.Session).TotalMinutes <= UOL_CURRENCY_MINUTES_SESSION_TIME;
        }
        #endregion
    }
}
