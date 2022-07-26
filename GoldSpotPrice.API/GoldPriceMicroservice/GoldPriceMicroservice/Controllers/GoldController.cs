using System.Threading.Tasks;
using GoldPriceMicroservice.Interfaces;
using GoldPriceMicroservice.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GoldPriceMicroservice.Controllers
{
    [ApiController]
    [Route("api/gold")]
    public class GoldController : ControllerBase
    {
        private readonly IExternalPriceCalloutService _priceService;

        public GoldController(IExternalPriceCalloutService spotPriceService)
        {
            _priceService = spotPriceService;
        }

        [HttpGet("current_spot_price")]
        public async Task<ActionResult<GoldSpotPriceResonse>> GetTodaysSpotPriceOfGoldAsync()
        {
            var price = await _priceService.GetLatestGoldSpotPriceAsync();
            return price != null && price != string.Empty
                ? Ok(price)
                : Ok("Could not get latest spot price for gold."); 
        }
    }
}
