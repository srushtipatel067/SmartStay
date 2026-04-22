namespace SmartStay.API.DTOs.Room
{
    public class RoomAvailabilityDto
    {
        public int RoomId { get; set; }
        public int TotalRooms { get; set; }
        public int BookedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public bool IsAvailable { get; set; }
    }
}