using CountryCurrency.Crawler.Domain.Uol;
using System.Threading.Tasks;

namespace CountryCurrency.Crawler.Domain.Interface.Uol
{
    public interface IUolCurrencyCrawlerBuilder
    {
        bool CoutriesCurrencyMaxToReturnIsValid(string[] dataIds);
        Task<Domain.Uol.CountryCurrency[]> GetCountries();
        Task<CountryCurrencyData> BuildCountryCurrency(short dataId);
        Task<CountryCurrencyData[]> BuildCountriesCurrency(string[] dataIds);
    }
}
