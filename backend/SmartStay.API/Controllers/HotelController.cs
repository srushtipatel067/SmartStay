using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStay.API.DTOs.Common;
using SmartStay.API.DTOs.Hotel;
using SmartStay.API.Repositories.Interfaces;
using System.Security.Claims;

namespace SmartStay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto dto)
        {
            var validationError = ValidateCreateHotel(dto);
            if (validationError != null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = validationError,
                    Data = null
                });
            }

            var createdBy = GetCurrentUserId();

            var hotelId = await _hotelRepository.CreateHotelAsync(dto, createdBy);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Hotel created successfully.",
                Data = new
                {
                    HotelId = hotelId
                }
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _hotelRepository.GetAllHotelsAsync();

            return Ok(new ApiResponse<IEnumerable<HotelResponseDto>>
            {
                Success = true,
                Message = "Hotels fetched successfully.",
                Data = hotels
            });
        }

        [HttpGet("{hotelId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelById(int hotelId)
        {
            if (hotelId <= 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Valid HotelId is required.",
                    Data = null
                });
            }

            var hotel = await _hotelRepository.GetHotelByIdAsync(hotelId);

            if (hotel == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Hotel not found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<HotelResponseDto>
            {
                Success = true,
                Message = "Hotel fetched successfully.",
                Data = hotel
            });
        }

        [HttpPut("{hotelId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHotel(int hotelId, [FromBody] UpdateHotelDto dto)
        {
            if (hotelId <= 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Valid HotelId is required.",
                    Data = null
                });
            }

            if (dto == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Request body is required.",
                    Data = null
                });
            }

            // Route id should be the final source of truth
            dto.HotelId = hotelId;

            var validationError = ValidateUpdateHotel(dto);
            if (validationError != null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = validationError,
                    Data = null
                });
            }

            var existingHotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
            if (existingHotel == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Hotel not found.",
                    Data = null
                });
            }

            var updatedBy = GetCurrentUserId();

            var updated = await _hotelRepository.UpdateHotelAsync(dto, updatedBy);

            if (!updated)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Failed to update hotel.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Hotel updated successfully.",
                Data = null
            });
        }

        [HttpDelete("{hotelId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(int hotelId)
        {
            if (hotelId <= 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Valid HotelId is required.",
                    Data = null
                });
            }

            var existingHotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
            if (existingHotel == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Hotel not found.",
                    Data = null
                });
            }

            var updatedBy = GetCurrentUserId();

            var deleted = await _hotelRepository.DeleteHotelAsync(hotelId, updatedBy);

            if (!deleted)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Failed to delete hotel.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Hotel deleted successfully.",
                Data = null
            });
        }

        // Create-specific validation
        private string? ValidateCreateHotel(CreateHotelDto dto)
        {
            if (dto == null)
                return "Request body is required.";

            return ValidateCommonHotelFields(
                dto.HotelName,
                dto.AddressLine1,
                dto.City,
                dto.Country,
                dto.StarRating,
                dto.BasePricePerNight,
                dto.ContactEmail,
                dto.ContactPhone,
                dto.CheckInTime,
                dto.CheckOutTime
            );
        }

        // Update-specific validation
        private string? ValidateUpdateHotel(UpdateHotelDto dto)
        {
            if (dto == null)
                return "Request body is required.";

            if (dto.HotelId <= 0)
                return "Valid HotelId is required.";

            return ValidateCommonHotelFields(
                dto.HotelName,
                dto.AddressLine1,
                dto.City,
                dto.Country,
                dto.StarRating,
                dto.BasePricePerNight,
                dto.ContactEmail,
                dto.ContactPhone,
                dto.CheckInTime,
                dto.CheckOutTime
            );
        }

        // Reusable validation used by both Create and Update
        private string? ValidateCommonHotelFields(
            string hotelName,
            string addressLine1,
            string city,
            string country,
            decimal? starRating,
            decimal? basePricePerNight,
            string? contactEmail,
            string? contactPhone,
            TimeSpan? checkInTime,
            TimeSpan? checkOutTime)
        {
            if (string.IsNullOrWhiteSpace(hotelName))
                return "Hotel name is required.";

            if (hotelName.Trim().Length < 2)
                return "Hotel name must be at least 2 characters long.";

            if (string.IsNullOrWhiteSpace(addressLine1))
                return "AddressLine1 is required.";

            if (string.IsNullOrWhiteSpace(city))
                return "City is required.";

            if (string.IsNullOrWhiteSpace(country))
                return "Country is required.";

            if (starRating.HasValue && (starRating < 0 || starRating > 5))
                return "Star rating must be between 0 and 5.";

            if (basePricePerNight.HasValue && basePricePerNight < 0)
                return "Base price per night cannot be negative.";

            if (!string.IsNullOrWhiteSpace(contactEmail) && !IsValidEmail(contactEmail))
                return "Valid contact email is required.";

            if (!string.IsNullOrWhiteSpace(contactPhone) && !IsValidPhone(contactPhone))
                return "Valid contact phone is required.";

            if (checkInTime.HasValue && checkOutTime.HasValue && checkInTime == checkOutTime)
                return "Check-in time and check-out time cannot be the same.";

            return null;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            var cleanedPhone = phone.Trim();

            if (cleanedPhone.Length < 7 || cleanedPhone.Length > 15)
                return false;

            return cleanedPhone.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ');
        }

        // Reads current logged-in user id from JWT claims
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("UserID")?.Value
                              ?? User.FindFirst("userId")?.Value;

            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            return null;
        }
    }
}