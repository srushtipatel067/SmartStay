namespace SmartStay.API.DTOs.Package
{
    public class PackageDto
    {
        public int PackageId { get; set; }
        public int HotelId { get; set; }

        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}