namespace SmartStay.API.DTOs.Offer
{
    public class OfferDto
    {
        public int OfferId { get; set; }
        public string Title { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }

        public int? HotelId { get; set; }
        public int? RoomId { get; set; }
    }
}