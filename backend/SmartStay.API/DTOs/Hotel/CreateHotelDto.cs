namespace SmartStay.API.DTOs.Hotel
{
    public class CreateHotelDto
    {
        public string HotelName { get; set; }
        public string? HotelCode { get; set; }
        public string? Description { get; set; }

        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string? State { get; set; }
        public string Country { get; set; }
        public string? PostalCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public decimal? StarRating { get; set; }
        public decimal? BasePricePerNight { get; set; }

        public string? ThumbnailImageUrl { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
    }
}