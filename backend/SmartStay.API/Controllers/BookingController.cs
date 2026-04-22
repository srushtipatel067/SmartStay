using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStay.API.DTOs.Booking;
using SmartStay.API.DTOs.Common;
using SmartStay.API.Repositories.Interfaces;
using System.Security.Claims;
using System.Linq;

namespace SmartStay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpPost]
        [AllowAnonymous] // guest booking allowed
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {            
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                dto.UserId = GetUserId(); // associate booking with user if logged in

                // Auto fill details if not provided , Prevents null crash
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(dto.GuestEmail) && !string.IsNullOrWhiteSpace(email))
                    dto.GuestEmail = email;

                if (string.IsNullOrWhiteSpace(dto.GuestName) && !string.IsNullOrWhiteSpace(User.Identity?.Name))
                    dto.GuestName = User.Identity.Name;
            }
            else
            {
                dto.UserId = null;
            }

            // Basic validation
            var errors = ValidateBooking(dto);
            if (errors != null)
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors));

            var success = await _bookingRepository.CreateBookingAsync(dto);

            if (!success)
                return BadRequest(ApiResponse<string>.Fail("Booking failed!"));

            return Ok(ApiResponse<object>.Ok(new
            {
                Message = "Booking created successfully",
                PaymentInstruction = "Pay at property during check-in"
            }));
        }


        [HttpGet("my-bookings")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            int userId = GetUserId();

            var data = await _bookingRepository.GetUserBookingsAsync(userId);

            return Ok(ApiResponse<object>.Ok(data));
        }

        [HttpGet("guest-bookings")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGuestBookings(string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
                return BadRequest(ApiResponse<string>.Fail("Email and phone required"));

            var data = await _bookingRepository.GetGuestBookingsAsync(email, phone);

            return Ok(ApiResponse<object>.Ok(data));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _bookingRepository.GetAllBookingsAsync();

            return Ok(ApiResponse<object>.Ok(data));
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var allowed = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };

            if (!allowed.Contains(dto.Status, StringComparer.OrdinalIgnoreCase))
                return BadRequest(ApiResponse<string>.Fail("Invalid status"));

            var result = await _bookingRepository.UpdateStatusAsync(id, dto.Status);

            return result
                ? Ok(ApiResponse<string>.Ok(null, "Status updated."))
                : BadRequest(ApiResponse<string>.Fail("Update failed!"));
        }

        [HttpPut("{id}/payment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentDto dto)
        {
            var allowed = new[] { "Paid", "Refunded" };

            if (!allowed.Contains(dto.Status, StringComparer.OrdinalIgnoreCase))
                return BadRequest(ApiResponse<string>.Fail("Invalid payment status"));

            var result = await _bookingRepository.UpdatePaymentAsync(id, dto.Status);

            return result
                ? Ok(ApiResponse<string>.Ok(null, $"Payment marked {dto.Status}."))
                : BadRequest(ApiResponse<string>.Fail("Update failed!"));
        }

        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            //only admin or booking owner can cancel
            int? userId = null;
            bool isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
                userId = GetUserId();

            var result = await _bookingRepository.CancelBookingAsync(id, userId, isAdmin);

            return result
                ? Ok(ApiResponse<string>.Ok(null, "Booking cancelled."))
                : BadRequest(ApiResponse<string>.Fail("Cancellation failed!"));
        }

        // REUSABLE VALIDATION
        private Dictionary<string, string[]>? ValidateBooking(CreateBookingDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            void AddError(string key, string message)
            {
                if (!errors.ContainsKey(key))
                    errors[key] = new List<string>();

                errors[key].Add(message);
            }

            if (dto.CheckInDate.Date < DateTime.Now.Date) // check-in cannot be in the past
                AddError("checkInDate", "Check-in date cannot be in the past");

            if (dto.CheckInDate.Date == DateTime.Now.Date && DateTime.Now.Hour >= 18) // same-day booking not allowed after 6 PM
                AddError("checkInDate", "Same-day booking not allowed after 6 PM");

            if (dto.CheckInDate >= dto.CheckOutDate)
                AddError("date", "Invalid date range");

            if (dto.RoomsBooked <= 0)
                AddError("rooms", "Rooms must be at least 1");

            if (string.IsNullOrWhiteSpace(dto.GuestName))
                AddError("guestName", "Guest name required");

            if (string.IsNullOrWhiteSpace(dto.GuestEmail))
                AddError("guestEmail", "Guest email required");

            if (string.IsNullOrWhiteSpace(dto.GuestPhone))
                AddError("guestPhone", "Guest phone required");

            return errors.Any()
                ? errors.ToDictionary(x => x.Key, x => x.Value.ToArray())
                : null;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}