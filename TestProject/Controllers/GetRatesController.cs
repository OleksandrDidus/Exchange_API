using Microsoft.AspNetCore.Mvc;
using TestProject.Interface;
using TestProject.Service;

namespace TestProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetRatesController : Controller
    {
        private readonly IExchangeRateService _exchangeRateService;

        public GetRatesController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRates(string baseCurrency, string quoteCurrency)
        {
            var binanceRate = await _exchangeRateService.GetRateAsync("binance", baseCurrency, quoteCurrency);
            var kucoinRate = await _exchangeRateService.GetRateAsync("kucoin", baseCurrency, quoteCurrency);

            return Ok(new[]
            {
            new { exchangeName = "binance", rate = binanceRate },
            new { exchangeName = "kucoin", rate = kucoinRate }
        });
        }

        [Route("/getRates")]
        public IActionResult Index()
        {
            return View();
        }
    }


}
