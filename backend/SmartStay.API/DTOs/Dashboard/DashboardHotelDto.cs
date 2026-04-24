namespace SmartStay.API.DTOs.Dashboard
{
    public class DashboardHotelDto
    {
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string City { get; set; }
        public decimal? StarRating { get; set; }

        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerEmail { get; set; }
    }
}