using Dapper;
using Microsoft.Data.SqlClient;
using SmartStay.API.DTOs.Review;
using SmartStay.API.Repositories.Interfaces;
using System.Data;

namespace SmartStay.API.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly string _connectionString;

        public ReviewRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<(bool Success, string Message)> CreateReviewAsync(int userId, CreateReviewDto dto)
        {
            using var conn = Connection;

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Review_Create",
                new
                {
                    BookingId = dto.BookingId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Comment = dto.Comment
                },
                commandType: CommandType.StoredProcedure
            );

            return result != null
                ? ((bool)(result.Success == 1), (string)result.Message)
                : (false, "Unknown error");
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByHotelAsync(int hotelId)
        {
            using var conn = Connection;

            return await conn.QueryAsync<ReviewDto>(
                "sp_Review_GetByHotel",
                new 
                { HotelId = hotelId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}