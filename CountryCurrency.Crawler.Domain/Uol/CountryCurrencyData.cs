namespace CountryCurrency.Crawler.Domain.Uol
{
    public class CountryCurrencyData
    {
        public CountryCurrencyData(CountryCurrency countryCurrency, CurrencyDataDoc currencyDataDoc) 
        {
            DataId = countryCurrency.DataId;
            CountryCode = countryCurrency.CountryCode;
            CountryName = countryCurrency.CountryName;
            CurrencyDataDoc = currencyDataDoc;
        }
        public short DataId { get; }
        public string CountryCode { get; }
        public string CountryName { get; }
        public CurrencyDataDoc CurrencyDataDoc { get; }
    }
}
