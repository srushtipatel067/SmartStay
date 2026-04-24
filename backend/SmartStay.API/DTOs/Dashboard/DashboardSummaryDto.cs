namespace SmartStay.API.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalHotels { get; set; }
        public int TotalRooms { get; set; }
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}