using System.Threading.Tasks;

namespace GoldPriceMicroservice.Interfaces
{
    public interface IExternalPriceCalloutService
    {
        Task<string> GetLatestGoldSpotPriceAsync();
    }
}
