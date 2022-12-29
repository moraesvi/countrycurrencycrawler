namespace Crawler.Utils.Domain.Infra.CountryCurrency
{
    public class CountryCurrencyDataDb
    {
        public short DataId { get; }
        public string CountryCode { get; }
        public string CountryName { get; }
        public CurrencyDataDocDb CurrencyDataDoc { get; }
    }
}
