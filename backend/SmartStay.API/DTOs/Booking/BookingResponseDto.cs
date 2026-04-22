namespace SmartStay.API.DTOs.Booking
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public string GuestName { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
    }
}