using Dapper;
using Microsoft.Data.SqlClient;
using SmartStay.API.DTOs.Hotel;
using SmartStay.API.Repositories.Interfaces;
using System.Data;

namespace SmartStay.API.Repositories.Implementations
{
    public class HotelRepository : IHotelRepository
    {
        private readonly string _connectionString;

        public HotelRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<int> CreateHotelAsync(CreateHotelDto dto, int? createdBy)
        {
            using var conn = Connection;

            var hotelId = await conn.ExecuteScalarAsync<int>(
                "sp_Hotel_Create",
                new
                {
                    dto.HotelName,
                    dto.HotelCode,
                    dto.Description,

                    dto.AddressLine1,
                    dto.AddressLine2,
                    dto.City,
                    dto.State,
                    dto.Country,
                    dto.PostalCode,

                    dto.Latitude,
                    dto.Longitude,

                    dto.StarRating,
                    dto.BasePricePerNight,

                    dto.ThumbnailImageUrl,
                    dto.ContactEmail,
                    dto.ContactPhone,

                    dto.CheckInTime,
                    dto.CheckOutTime,

                    CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure
            );

            return hotelId;
        }

        public async Task<IEnumerable<HotelResponseDto>> GetAllHotelsAsync()
        {
            using var conn = Connection;

            return await conn.QueryAsync<HotelResponseDto>(
                "sp_Hotel_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<HotelResponseDto?> GetHotelByIdAsync(int hotelId)
        {
            using var conn = Connection;

            return await conn.QueryFirstOrDefaultAsync<HotelResponseDto>(
                "sp_Hotel_GetById",
                new { HotelId = hotelId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> UpdateHotelAsync(UpdateHotelDto dto, int? updatedBy)
        {
            using var conn = Connection;

            var rowsAffected = await conn.ExecuteScalarAsync<int>(
                "sp_Hotel_Update",
                new
                {
                    dto.HotelId,
                    dto.HotelName,
                    dto.HotelCode,
                    dto.Description,

                    dto.AddressLine1,
                    dto.AddressLine2,
                    dto.City,
                    dto.State,
                    dto.Country,
                    dto.PostalCode,

                    dto.Latitude,
                    dto.Longitude,

                    dto.StarRating,
                    dto.BasePricePerNight,

                    dto.ThumbnailImageUrl,
                    dto.ContactEmail,
                    dto.ContactPhone,

                    dto.CheckInTime,
                    dto.CheckOutTime,

                    dto.IsActive,
                    UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteHotelAsync(int hotelId, int? updatedBy)
        {
            using var conn = Connection;

            var rowsAffected = await conn.ExecuteScalarAsync<int>(
                "sp_Hotel_Delete",
                new
                {
                    HotelId = hotelId,
                    UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }
    }
}