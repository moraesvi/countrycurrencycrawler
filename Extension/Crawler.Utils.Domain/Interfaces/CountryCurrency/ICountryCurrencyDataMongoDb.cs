using Crawler.Utils.Domain.Infra.CountryCurrency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Utils.Domain.Interfaces.CountryCurrency
{
    public interface ICountryCurrencyDataMongoDb
    {
        Task<List<CountryCurrencyDataDb>> Get();
        Task<CountryCurrencyDataDb> Add(CountryCurrencyDataDb model);
        Task RemoveAll();
    }
}
