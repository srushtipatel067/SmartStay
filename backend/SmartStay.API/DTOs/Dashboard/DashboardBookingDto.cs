namespace SmartStay.API.DTOs.Dashboard
{
    public class DashboardBookingDto
    {
        public int BookingId { get; set; }
        public string GuestName { get; set; }
        public string HotelName { get; set; }
        public string RoomType { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}