using Newtonsoft.Json;
using TestProject.Interface;

namespace TestProject.Service
{
    public class BinanceApiClient : IExchangeApiClient
    {
        private readonly HttpClient _httpClient;

        public BinanceApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency)
        {
            // Спочатку пробуємо формат base-quote (BTCUSDT)
            string symbol = $"{baseCurrency}{quoteCurrency}".ToUpper();
            var response = await _httpClient.GetAsync($"/api/v3/ticker/price?symbol={symbol}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<BinancePriceResponse>(json);
                return data.Price;
            }

            // Якщо запит не вдався, пробуємо інший напрямок (USDTBTC)
            symbol = $"{quoteCurrency}{baseCurrency}".ToUpper();
            response = await _httpClient.GetAsync($"/api/v3/ticker/price?symbol={symbol}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<BinancePriceResponse>(json);
                return 1 / data.Price; // Інвертуємо курс
            }

            throw new HttpRequestException($"Error fetching data from Binance for {baseCurrency}/{quoteCurrency}");
        }
    }



    public class BinancePriceResponse
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }
    }


    public class KucoinApiClient : IExchangeApiClient
    {
        private readonly HttpClient _httpClient;

        public KucoinApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency)
        {
            string symbol = $"{baseCurrency}-{quoteCurrency}".ToUpper();
            var response = await _httpClient.GetAsync($"/api/v1/market/orderbook/level1?symbol={symbol}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<KucoinLevel1Response>(json);
                if (data?.Data?.Price.HasValue == true)
                {
                    return data.Data.Price.Value;
                }
            }

            symbol = $"{quoteCurrency}-{baseCurrency}".ToUpper();
            response = await _httpClient.GetAsync($"/api/v1/market/orderbook/level1?symbol={symbol}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<KucoinLevel1Response>(json);
                if (data?.Data?.Price.HasValue == true)
                {
                    return 1 / data.Data.Price.Value; // Інвертуємо курс
                }
            }

            throw new HttpRequestException($"No valid data found on KuCoin for {baseCurrency}/{quoteCurrency}");
        }
    }

    public class KucoinLevel1Response
    {
        public KucoinData Data { get; set; }
    }

    public class KucoinData
    {
        public decimal? Price { get; set; }
    }

    public class KucoinPriceResponse
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
