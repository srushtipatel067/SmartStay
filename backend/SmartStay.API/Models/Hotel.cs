namespace SmartStay.API.Models
{
    public class Hotel
    {
        public int HotelId { get; set; }

        public string HotelName { get; set; } = string.Empty;
        public string? HotelCode { get; set; }
        public string? Description { get; set; }

        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public decimal? StarRating { get; set; }
        public decimal? AverageRating { get; set; }
        public decimal? BasePricePerNight { get; set; }

        public string? ThumbnailImageUrl { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}