namespace SmartStay.API.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        public int HotelId { get; set; }

        public string RoomType { get; set; } = string.Empty;
        public string? Description { get; set; }

        public decimal PricePerNight { get; set; }

        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }

        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }

        public string? ThumbnailImageUrl { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}