using Dapper;
using Microsoft.Data.SqlClient;
using SmartStay.API.DTOs.Package;
using SmartStay.API.Repositories.Interfaces;
using System.Data;

namespace SmartStay.API.Repositories.Implementations
{
    public class PackageRepository : IPackageRepository
    {
        private readonly string _connectionString;

        public PackageRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<(bool Success, string Message)> CreatePackageAsync(CreatePackageDto dto)
        {
            using var conn = Connection;

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Package_Create",
                new
                {
                    dto.HotelId,
                    dto.Title,
                    dto.Description,
                    dto.Price,
                    dto.StartDate,
                    dto.EndDate
                },
                commandType: CommandType.StoredProcedure
            );

            return result != null
                ? ((bool)(result.Success == 1), (string)result.Message)
                : (false, "Package creation failed");
        }

        public async Task<IEnumerable<PackageDto>> GetPackagesByHotelAsync(int hotelId)
        {
            using var conn = Connection;

            return await conn.QueryAsync<PackageDto>(
                "sp_Package_GetByHotel",
                new { HotelId = hotelId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}