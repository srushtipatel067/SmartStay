namespace SmartStay.API.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
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

        public decimal PricePerNight { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalAmount { get; set; }

        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
