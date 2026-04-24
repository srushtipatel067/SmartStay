using SmartStay.API.DTOs.Dashboard;

namespace SmartStay.API.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<IEnumerable<DashboardHotelDto>> GetHotelsAsync();
        Task<IEnumerable<DashboardBookingDto>> GetRecentBookingsAsync();
    }
}