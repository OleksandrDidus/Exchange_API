namespace TestProject.Interface
{
    public interface IExchangeApiClient
    {
        Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency);
    }
}
