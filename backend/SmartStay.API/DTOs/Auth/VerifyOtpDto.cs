using System.ComponentModel.DataAnnotations;

namespace SmartStay.API.DTOs.Auth
{
    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
        public string OtpCode { get; set; } = string.Empty;
    }
}