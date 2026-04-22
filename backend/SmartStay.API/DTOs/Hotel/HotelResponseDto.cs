namespace SmartStay.API.DTOs.Hotel
{
    public class HotelResponseDto
    {
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string? Description { get; set; }

        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public decimal? StarRating { get; set; }
        public decimal? AverageRating { get; set; }
        public decimal? BasePricePerNight { get; set; }

        public string? ThumbnailImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}