using SmartStay.API.DTOs.Room;

namespace SmartStay.API.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        Task<int?> CreateAsync(CreateRoomDto dto, int createdBy);
        Task<IEnumerable<RoomResponseDto>> GetAllAsync();
        Task<RoomResponseDto?> GetByIdAsync(int roomId);
        Task<IEnumerable<RoomResponseDto>> GetByHotelIdAsync(int hotelId);
        Task<RoomAvailabilityDto?> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<bool> UpdateAsync(UpdateRoomDto dto, int updatedBy);
        Task<bool> DeleteAsync(int roomId, int updatedBy);
    }
}