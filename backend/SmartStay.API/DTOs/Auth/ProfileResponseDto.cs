namespace SmartStay.API.DTOs.Auth
{
    public class ProfileResponseDto
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? ProfileImage { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}