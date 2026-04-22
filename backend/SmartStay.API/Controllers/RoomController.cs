using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStay.API.DTOs.Room;
using SmartStay.API.Repositories.Interfaces;
using System.Security.Claims;

namespace SmartStay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        // CREATE ROOM
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            var validationError = ValidateCreate(dto);
            if (validationError != null)
                return BadRequest(validationError);

            var userId = GetUserId();

            var roomId = await _roomRepository.CreateAsync(dto, userId);

            if (roomId == null)
                return BadRequest("Failed to create room.");

            return Ok(new
            {
                Message = "Room created successfully.",
                RoomId = roomId
            });
        }

        // GET ALL ROOMS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return Ok(rooms);
        }

        // GET ROOM BY ID
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetById(int roomId)
        {
            if (roomId <= 0)
                return BadRequest("Invalid RoomId.");

            var room = await _roomRepository.GetByIdAsync(roomId);

            if (room == null)
                return NotFound("Room not found.");

            return Ok(room);
        }

        // GET ROOMS BY HOTEL
        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetByHotelId(int hotelId)
        {
            if (hotelId <= 0)
                return BadRequest("Invalid HotelId.");

            var rooms = await _roomRepository.GetByHotelIdAsync(hotelId);

            return Ok(rooms);
        }

        // UPDATE ROOM
        [HttpPut("{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int roomId, [FromBody] UpdateRoomDto dto)
        {
            if (roomId != dto.RoomId)
                return BadRequest("RoomId mismatch.");

            var validationError = ValidateUpdate(dto);
            if (validationError != null)
                return BadRequest(validationError);

            var userId = GetUserId();

            var result = await _roomRepository.UpdateAsync(dto, userId);

            if (!result)
                return NotFound("Room not found or update failed.");

            return Ok("Room updated successfully.");
        }

        // DELETE ROOM
        [HttpDelete("{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int roomId)
        {
            if (roomId <= 0)
                return BadRequest("Invalid RoomId.");

            var userId = GetUserId();

            var result = await _roomRepository.DeleteAsync(roomId, userId);

            if (!result)
                return NotFound("Room not found.");

            return Ok("Room deleted successfully.");
        }

        // Reusable Validations
        private string? ValidateCreate(CreateRoomDto dto)
        {
            if (dto.HotelId <= 0) return "Valid HotelId is required.";
            if (string.IsNullOrWhiteSpace(dto.RoomType)) return "Room type is required.";
            if (dto.PricePerNight <= 0) return "Price must be greater than 0.";
            if (dto.MaxAdults <= 0) return "Max adults must be greater than 0.";
            if (dto.TotalRooms <= 0) return "Total rooms must be greater than 0.";
            if (dto.AvailableRooms < 0) return "Available rooms cannot be negative.";
            if (dto.AvailableRooms > dto.TotalRooms) return "Available rooms cannot exceed total rooms.";

            return null;
        }

        private string? ValidateUpdate(UpdateRoomDto dto)
        {
            if (dto.RoomId <= 0) return "Invalid RoomId.";
            return ValidateCreate(new CreateRoomDto
            {
                HotelId = dto.HotelId,
                RoomType = dto.RoomType,
                PricePerNight = dto.PricePerNight,
                MaxAdults = dto.MaxAdults,
                TotalRooms = dto.TotalRooms,
                AvailableRooms = dto.AvailableRooms
            });
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}