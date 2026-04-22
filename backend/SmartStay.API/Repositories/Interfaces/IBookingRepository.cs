using SmartStay.API.DTOs.Booking;

namespace SmartStay.API.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> CreateBookingAsync(CreateBookingDto dto);
        Task<IEnumerable<dynamic>> GetAllBookingsAsync();
        Task<IEnumerable<dynamic>> GetUserBookingsAsync(int userId);
        Task<IEnumerable<dynamic>> GetGuestBookingsAsync(string email, string phone);

        Task<bool> UpdateStatusAsync(int bookingId, string status);
        Task<bool> UpdatePaymentAsync(int bookingId, string status);
        Task<bool> CancelBookingAsync(int bookingId, int? userId, bool isAdmin);
    }
}