using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using SmartStay.API.Repositories.Interfaces;
using SmartStay.API.DTOs.Booking;

namespace SmartStay.API.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<bool> CreateBookingAsync(CreateBookingDto dto)
        {
            using var conn = Connection;

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Booking_Create",
                new
                {
                    dto.UserId,
                    dto.GuestName,
                    dto.GuestEmail,
                    dto.GuestPhone,
                    dto.HotelId,
                    dto.RoomId,
                    dto.CheckInDate,
                    dto.CheckOutDate,
                    dto.Adults,
                    dto.Children,
                    dto.RoomsBooked,
                    dto.SpecialRequest
                },
                commandType: CommandType.StoredProcedure
            );

            return result != null && result.Success == 1;
        }

        public async Task<IEnumerable<dynamic>> GetAllBookingsAsync()
        {
            using var conn = Connection;

            return await conn.QueryAsync(
                "sp_Booking_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<dynamic>> GetUserBookingsAsync(int userId)
        {
            using var conn = Connection;

            return await conn.QueryAsync(
                "sp_Booking_GetByUser",
                new 
                { UserId = userId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<dynamic>> GetGuestBookingsAsync(string email, string phone)
        {
            using var conn = Connection;

            return await conn.QueryAsync(
                "sp_Booking_GetByGuest",
                new 
                {
                    GuestEmail = email, 
                    GuestPhone = phone 
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> UpdateStatusAsync(int bookingId, string status)
        {
            using var conn = Connection;

            var rows = await conn.ExecuteScalarAsync<int>(
                "sp_Booking_UpdateStatus",
                new 
                { 
                    BookingId = bookingId, 
                    Status = status 
                },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        public async Task<bool> UpdatePaymentAsync(int bookingId, string status)
        {
            using var conn = Connection;

            var rows = await conn.ExecuteScalarAsync<int>(
                "sp_Booking_UpdatePayment",
                new 
                {
                    BookingId = bookingId ,
                    Status = status
                },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int? userId, bool isAdmin)
        {
            using var conn = Connection;

            var rows = await conn.ExecuteScalarAsync<int>(
                "sp_Booking_Cancel",
                new
                {
                    BookingId = bookingId,
                    UserId = userId,
                    IsAdmin = isAdmin
                },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }
    }
}