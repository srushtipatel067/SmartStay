namespace SmartStay.API.DTOs.Booking
{
    public class CreateBookingDto
    {
        public int? UserId { get; set; }

        public string GuestName { get; set; }
        public string GuestEmail { get; set; }
        public string GuestPhone { get; set; }

        public int HotelId { get; set; }
        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int Adults { get; set; }
        public int Children { get; set; }

        public int RoomsBooked { get; set; }

        public string? SpecialRequest { get; set; }
    }
}