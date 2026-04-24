namespace SmartStay.API.DTOs.Package
{
    public class CreatePackageDto
    {
        public int HotelId { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}