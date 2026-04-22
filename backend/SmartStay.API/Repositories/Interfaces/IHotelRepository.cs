using SmartStay.API.DTOs.Hotel;

namespace SmartStay.API.Repositories.Interfaces
{
    public interface IHotelRepository
    {
        Task<int> CreateHotelAsync(CreateHotelDto dto, int? createdBy);
        Task<IEnumerable<HotelResponseDto>> GetAllHotelsAsync();
        Task<HotelResponseDto?> GetHotelByIdAsync(int hotelId);
        Task<bool> UpdateHotelAsync(UpdateHotelDto dto, int? updatedBy);
        Task<bool> DeleteHotelAsync(int hotelId, int? updatedBy);
    }
}