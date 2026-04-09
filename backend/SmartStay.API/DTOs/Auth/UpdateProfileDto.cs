using System.ComponentModel.DataAnnotations;

namespace SmartStay.API.DTOs.Auth
{
    public class UpdateProfileDto
    {
        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}