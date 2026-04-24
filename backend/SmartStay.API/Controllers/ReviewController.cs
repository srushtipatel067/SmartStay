using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStay.API.DTOs.Common;
using SmartStay.API.DTOs.Review;
using SmartStay.API.Repositories.Interfaces;
using System.Security.Claims;

namespace SmartStay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // POST: api/review
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(CreateReviewDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _reviewRepository.CreateReviewAsync(userId, dto);

            return result.Success
                ? Ok(ApiResponse<string>.Ok(null, result.Message))
                : BadRequest(ApiResponse<string>.Fail(result.Message));
        }

        // GET: api/review/hotel/1
        [HttpGet("hotel/{hotelId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByHotel(int hotelId)
        {
            var data = await _reviewRepository.GetReviewsByHotelAsync(hotelId);

            return Ok(ApiResponse<object>.Ok(data));
        }
    }
}