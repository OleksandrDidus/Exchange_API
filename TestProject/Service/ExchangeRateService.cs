using TestProject.Interface;
using static TestProject.Service.ExchangeRateService;

namespace TestProject.Service
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly BinanceApiClient _binanceClient;
        private readonly KucoinApiClient _kucoinClient;

        public ExchangeRateService(BinanceApiClient binanceClient, KucoinApiClient kucoinClient)
        {
            _binanceClient = binanceClient;
            _kucoinClient = kucoinClient;
        }

        public async Task<decimal> GetRateAsync(string exchangeName, string baseCurrency, string quoteCurrency)
        {
            return exchangeName.ToLower() switch
            {
                "binance" => await _binanceClient.GetRateAsync(baseCurrency, quoteCurrency),
                "kucoin" => await _kucoinClient.GetRateAsync(baseCurrency, quoteCurrency),
                _ => throw new ArgumentException("Unsupported exchange")
            };
        }

        public async Task<EstimateResult> EstimateExchangeAsync(decimal inputAmount, string inputCurrency, string outputCurrency)
        {
            try
            {
                var binanceRate = await _binanceClient.GetRateAsync(inputCurrency, outputCurrency);
                var kucoinRate = await _kucoinClient.GetRateAsync(inputCurrency, outputCurrency);

                var binanceOutput = inputAmount * binanceRate;
                var kucoinOutput = inputAmount * kucoinRate;

                if (binanceOutput > kucoinOutput)
                {
                    return new EstimateResult { BestExchange = "binance", OutputAmount = binanceOutput };
                }
                else
                {
                    return new EstimateResult { BestExchange = "kucoin", OutputAmount = kucoinOutput };
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error occurred while estimating exchange", ex);
            }
        }

    }



}
