using SmartStay.API.DTOs.Offer;

namespace SmartStay.API.Repositories.Interfaces
{
    public interface IOfferRepository
    {
        Task<(bool Success, string Message)> CreateOfferAsync(CreateOfferDto dto);
        Task<IEnumerable<OfferDto>> GetActiveOffersAsync();
    }
}