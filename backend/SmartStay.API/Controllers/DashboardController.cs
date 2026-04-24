using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStay.API.DTOs.Common;
using SmartStay.API.Repositories.Interfaces;

namespace SmartStay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // only admin
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        // GET: api/dashboard/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var data = await _dashboardRepository.GetSummaryAsync();

            return Ok(ApiResponse<object>.Ok(data));
        }

        // GET: api/dashboard/hotels
        [HttpGet("hotels")]
        public async Task<IActionResult> GetHotels()
        {
            var data = await _dashboardRepository.GetHotelsAsync();

            return Ok(ApiResponse<object>.Ok(data));
        }

        // GET: api/dashboard/recent-bookings
        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings()
        {
            var data = await _dashboardRepository.GetRecentBookingsAsync();

            return Ok(ApiResponse<object>.Ok(data));
        }
    }
}