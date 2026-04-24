using Dapper;
using Microsoft.Data.SqlClient;
using SmartStay.API.DTOs.Offer;
using SmartStay.API.Repositories.Interfaces;
using System.Data;

namespace SmartStay.API.Repositories.Implementations
{
    public class OfferRepository : IOfferRepository
    {
        private readonly string _connectionString;

        public OfferRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<(bool Success, string Message)> CreateOfferAsync(CreateOfferDto dto)
        {
            using var conn = Connection;

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Offer_Create",
                new
                {
                    dto.Title,
                    dto.Description,
                    dto.DiscountType,
                    dto.DiscountValue,
                    dto.HotelId,
                    dto.RoomId,
                    dto.StartDate,
                    dto.EndDate
                },
                commandType: CommandType.StoredProcedure
            );

            return result != null
                ? ((bool)(result.Success == 1), (string)result.Message)
                : (false, "Offer creation failed");
        }

        public async Task<IEnumerable<OfferDto>> GetActiveOffersAsync()
        {
            using var conn = Connection;

            return await conn.QueryAsync<OfferDto>(
                "sp_Offer_GetActive",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}