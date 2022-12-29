using CountryCurrency.Crawler.Common;
using CountryCurrency.Crawler.Domain.Interface.Uol;
using CountryCurrency.Crawler.Domain.Uol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CountryCurrency.Crawler.WebApi.Controllers
{
    [Route("api/countrycurrency")]
    [ApiController]
    public class CountryCurrencyController : CustomControllerBase
    {
        private readonly ILogger<CountryCurrencyController> _logger;
        private readonly IUolCurrencyCrawlerBuilder _currencyCrawlerBuilder;
        public CountryCurrencyController(ILogger<CountryCurrencyController> logger, IUolCurrencyCrawlerBuilder currencyCrawlerBuilder)
            : base(logger)
        {
            _logger = logger;
            _currencyCrawlerBuilder = currencyCrawlerBuilder;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            _logger.LogInformation("Executou busca de países {GET}");

            Domain.Uol.CountryCurrency[] countries = await _currencyCrawlerBuilder.GetCountries();
            return OkResult(countries);
        }

        [HttpGet("{dataId}")]
        public async Task<ActionResult> Currency(short dataId)
        {
            _logger.LogInformation("Executou busca de cotação {Currency}");

            CountryCurrencyData countryCurrency = await _currencyCrawlerBuilder.BuildCountryCurrency(dataId);
            return OkResult(countryCurrency);
        }

        [HttpGet("{dataId}/array")]
        public async Task<ActionResult> Currency(string dataId)
        {
            if (string.IsNullOrEmpty(dataId))
                return ForbidResult("Obrigatório informar o {dataId} do país");

            string[] dataIds = dataId.Split('|')
                                    ?.ToArray() ?? new string[] { };

            if (dataIds.Count() == 0 || !_currencyCrawlerBuilder.CoutriesCurrencyMaxToReturnIsValid(dataIds))
                return ForbidResult($"O {dataId} informado é inválido");

            _logger.LogInformation("Executou busca de cotação {Currency/Array}");

            CountryCurrencyData[] countryCurrency = await _currencyCrawlerBuilder.BuildCountriesCurrency(dataIds);
            return OkResult(countryCurrency);
        }
    }
}
