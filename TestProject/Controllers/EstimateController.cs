using Microsoft.AspNetCore.Mvc;
using TestProject.Interface;
using TestProject.Service;

namespace TestProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstimateController : Controller
    {
        private readonly IExchangeRateService _exchangeRateService;

        public EstimateController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet/*("estimate")*/]
        public async Task<IActionResult> EstimateExchange([FromQuery] decimal inputAmount, [FromQuery] string inputCurrency, [FromQuery] string outputCurrency)
        {
            try
            {
                var result = await _exchangeRateService.EstimateExchangeAsync(inputAmount, inputCurrency, outputCurrency);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Route("/estimate")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
