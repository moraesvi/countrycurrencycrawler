using CountryCurrency.Crawler.Domain.Uol;

namespace CountryCurrency.Crawler.Parse
{
    public interface ICountriesParse
    {
        Domain.Uol.CountryCurrency[] Parse(string html);        
    }
}
