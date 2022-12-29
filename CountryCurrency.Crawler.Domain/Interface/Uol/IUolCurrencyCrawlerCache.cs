using CountryCurrency.Crawler.Domain.Uol;
using System.Threading.Tasks;

namespace CountryCurrency.Crawler.Domain.Interface.Uol
{
    public interface IUolCurrencyCrawlerCache
    {
        Task<Domain.Uol.CountryCurrency[]> GetCountries();
        Task<CountryCurrencyData> GetCountryCurrencyData(Domain.Uol.CountryCurrency countryCurrency);
        Task<CountryCurrencyData[]> GetCountriesCurrencyData(Domain.Uol.CountryCurrency[] countriesCurrency);
    }
}
