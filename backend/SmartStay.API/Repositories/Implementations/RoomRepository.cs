using Dapper;
using Microsoft.Data.SqlClient;
using SmartStay.API.DTOs.Room;
using SmartStay.API.Repositories.Interfaces;
using System.Data;

namespace SmartStay.API.Repositories.Implementations
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;

        public RoomRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        // CREATE
        public async Task<int?> CreateAsync(CreateRoomDto dto, int createdBy)
        {
            using var conn = Connection;

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Room_Create",
                new
                {
                    dto.HotelId,
                    dto.RoomType,
                    dto.Description,
                    dto.PricePerNight,
                    dto.MaxAdults,
                    dto.MaxChildren,
                    dto.TotalRooms,
                    dto.AvailableRooms,
                    dto.ThumbnailImageUrl,
                    CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.Success == 0)
                return null;

            return (int?)result.RoomId;
        }

        // GET ALL
        public async Task<IEnumerable<RoomResponseDto>> GetAllAsync()
        {
            using var conn = Connection;

            return await conn.QueryAsync<RoomResponseDto>(
                "sp_Room_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // GET BY ID
        public async Task<RoomResponseDto?> GetByIdAsync(int roomId)
        {
            using var conn = Connection;

            return await conn.QueryFirstOrDefaultAsync<RoomResponseDto>(
                "sp_Room_GetById",
                new { RoomId = roomId },
                commandType: CommandType.StoredProcedure
            );
        }

        // GET BY HOTEL
        public async Task<IEnumerable<RoomResponseDto>> GetByHotelIdAsync(int hotelId)
        {
            using var conn = Connection;

            return await conn.QueryAsync<RoomResponseDto>(
                "sp_Room_GetByHotelId",
                new { HotelId = hotelId },
                commandType: CommandType.StoredProcedure
            );
        }

        // UPDATE
        public async Task<bool> UpdateAsync(UpdateRoomDto dto, int updatedBy)
        {
            using var conn = Connection;

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Room_Update",
                new
                {
                    dto.RoomId,
                    dto.HotelId,
                    dto.RoomType,
                    dto.Description,
                    dto.PricePerNight,
                    dto.MaxAdults,
                    dto.MaxChildren,
                    dto.TotalRooms,
                    dto.AvailableRooms,
                    dto.ThumbnailImageUrl,
                    dto.IsActive,
                    UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.Success == 0)
                return false;

            return true;
        }

        // DELETE (soft delete)
        public async Task<bool> DeleteAsync(int roomId, int updatedBy)
        {
            using var conn = Connection;

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Room_Delete",
                new
                {
                    RoomId = roomId,
                    UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.Success == 0)
                return false;

            return true;
        }
    }
}