namespace Crawler.Utils.Domain.Infra.CountryCurrency
{
    public class CurrencyDataDocDb
    {
        public double BidValue { get; set; }
        public double AskValue { get; set; }
        public double MaxBid { get; set; }
        public double MinBid { get; set; }
        public double VariationBid { get; set; }
        public double VariationpercentBid { get; set; }
        public double OpenbidValue { get; set; }
        public string Date { get; set; }
    }
}
