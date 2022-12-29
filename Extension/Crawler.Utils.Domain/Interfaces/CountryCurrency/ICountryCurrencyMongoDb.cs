using Crawler.Utils.Domain.Infra.CountryCurrency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Utils.Domain.Interfaces.CountryCurrency
{
    interface ICountryCurrencyMongoDb
    {
        Task<List<CountryCurrencyDb>> Get();
        Task<CountryCurrencyDb> Add(CountryCurrencyDb model);
        Task RemoveAll();
    }
}
