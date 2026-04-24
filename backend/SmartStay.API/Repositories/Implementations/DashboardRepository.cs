using Dapper;
using Microsoft.Data.SqlClient;
using SmartStay.API.DTOs.Dashboard;
using SmartStay.API.Repositories.Interfaces;
using System.Data;

namespace SmartStay.API.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            using var conn = Connection;

            return await conn.QueryFirstAsync<DashboardSummaryDto>(
                "sp_Dashboard_Summary",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<DashboardHotelDto>> GetHotelsAsync()
        {
            using var conn = Connection;

            return await conn.QueryAsync<DashboardHotelDto>(
                "sp_Dashboard_HotelsWithManager",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<DashboardBookingDto>> GetRecentBookingsAsync()
        {
            using var conn = Connection;

            return await conn.QueryAsync<DashboardBookingDto>(
                "sp_Dashboard_RecentBookings",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}