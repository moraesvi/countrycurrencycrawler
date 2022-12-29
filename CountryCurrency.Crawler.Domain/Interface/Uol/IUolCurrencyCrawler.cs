using CountryCurrency.Crawler.Domain.Uol;
using System;
using System.Threading.Tasks;

namespace CountryCurrency.Crawler.Domain.Interface.Uol
{
    public interface IUolCurrencyCrawler : IDisposable
    {
        bool CoutriesCurrencyMaxToReturnIsValid(string[] dataIds);
        bool CoutriesCurrencyMaxToReturnIsValid(Domain.Uol.CountryCurrency[] dataIds);
        Task<Domain.Uol.CountryCurrency[]> GetCountries();
        Task<CountryCurrencyData> GetCountryCurrencyData(Domain.Uol.CountryCurrency countryCurrency);
        Task<CountryCurrencyData[]> GetCountriesCurrencyData(Domain.Uol.CountryCurrency[] countriesCurrency);
    }
}
