using Crawler.Utils.Domain.Infra.CountryCurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Utils.Domain.Interfaces.CountryCurrency
{
    public interface ICurrencyDataDocMongoDb
    {
        Task<List<CurrencyDataDocDb>> Get();
        Task<CurrencyDataDocDb> Add(CurrencyDataDocDb model);
        Task RemoveAll();
    }
}
