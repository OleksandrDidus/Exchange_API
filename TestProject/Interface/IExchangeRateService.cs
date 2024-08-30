namespace TestProject.Interface
{
    public interface IExchangeRateService
    {
        Task<decimal> GetRateAsync(string exchangeName, string baseCurrency, string quoteCurrency);
        Task<EstimateResult> EstimateExchangeAsync(decimal inputAmount, string inputCurrency, string outputCurrency);
    }
    public class EstimateResult
    {
        public string BestExchange { get; set; }
        public decimal OutputAmount { get; set; }
    }
}
