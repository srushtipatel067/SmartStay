using SmartStay.API.DTOs.Review;

namespace SmartStay.API.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<(bool Success, string Message)> CreateReviewAsync(int userId, CreateReviewDto dto);
        Task<IEnumerable<ReviewDto>> GetReviewsByHotelAsync(int hotelId);
    }
}