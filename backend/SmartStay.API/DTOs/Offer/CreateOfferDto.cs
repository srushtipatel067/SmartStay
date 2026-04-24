namespace SmartStay.API.DTOs.Offer
{
    public class CreateOfferDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }

        public string DiscountType { get; set; } // Percentage / Flat
        public decimal DiscountValue { get; set; }

        public int? HotelId { get; set; }
        public int? RoomId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}