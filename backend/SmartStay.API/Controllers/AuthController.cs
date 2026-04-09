using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartStay.API.DTOs.Auth;
using SmartStay.API.DTOs.Common;
using SmartStay.API.Helpers;
using SmartStay.API.Repositories.Interfaces;
using System.Security.Claims;

namespace SmartStay.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IAuthRepository authRepository, JwtHelper jwtHelper)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            // First let DTO validations run
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Validation failed", ModelState));

            // Normalize common text inputs
            dto.FullName = dto.FullName.Trim();
            dto.Email = dto.Email.Trim().ToLower();
            dto.PhoneNumber = dto.PhoneNumber.Trim();

            // Prevent meaningless name after trim
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest(ApiResponse<object>.Fail("Full name cannot be empty."));

            // Hidden protection against unrealistic long password payloads
            if (dto.Password.Length > 50)
                return BadRequest(ApiResponse<object>.Fail("Password is too long."));

            // Real-world password policy check
            if (!IsValidPassword(dto.Password))
            {
                return BadRequest(ApiResponse<object>.Fail(
                    "Password must include at least one uppercase letter, one lowercase letter, one number, and one special character."
                ));
            }

            // Allow realistic phone formats, reject clearly invalid ones
            if (!IsValidPhoneNumber(dto.PhoneNumber))
                return BadRequest(ApiResponse<object>.Fail("Enter a valid phone number."));

            var user = await _authRepository.RegisterAsync(dto);

            if (user == null)
                return BadRequest(ApiResponse<object>.Fail("Email already exists."));

            //Generate token only after successful registration
            var token = _jwtHelper.GenerateToken(user);

            var response = new AuthResponseDto
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Role = user.Role,
                Token = token
            };

            return Ok(ApiResponse<AuthResponseDto>.Ok(response, "User registered successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Validation failed", ModelState));

            // Clean email before checking DB
            dto.Email = dto.Email.Trim().ToLower();

            //Hidden protection against unrealistic long password payloads
            if(dto.Password.Length>50)
                return Unauthorized(ApiResponse<object>.Fail("Invalid credentials."));

            var result = await _authRepository.LoginAsync(dto);

            // Do not reveal whether email or password was incorrect
            if (result == null)
                return Unauthorized(ApiResponse<object>.Fail("Invalid credentials."));

            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful"));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Validation failed", ModelState));

            dto.Email = dto.Email.Trim().ToLower();

            var success = await _authRepository.ForgotPasswordAsync(dto);

            // Same response for both cases to avoid email existence leak
            return Ok(ApiResponse<object>.Ok(
                null,
                "If the email is registered, an OTP has been sent to the email address."
            ));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Validation failed", ModelState));

            dto.Email = dto.Email.Trim().ToLower();
            dto.OtpCode = dto.OtpCode.Trim();

            var success = await _authRepository.ResetPasswordAsync(dto);

            if (!success)
                return BadRequest(ApiResponse<object>.Fail("Invalid or expired OTP"));

            return Ok(ApiResponse<object>.Ok(null, "Password reset successfully"));
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Read logged-in user id from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token"));

            int userId = int.Parse(userIdClaim);

            var result = await _authRepository.GetProfileAsync(userId);

            if (result == null)
                return NotFound(ApiResponse<object>.Fail("User not found"));

            return Ok(ApiResponse<object>.Ok(result, "Profile fetched successfully"));
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token"));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Validation failed", ModelState));

            int userId = int.Parse(userIdClaim);

            // Optional practical trimming for text fields
            if (dto.FullName != null)
                dto.FullName = dto.FullName.Trim();

            if (dto.PhoneNumber != null)
                dto.PhoneNumber = dto.PhoneNumber.Trim();

            if (dto.Address != null)
                dto.Address = dto.Address.Trim();

            //Reject completely empty update request
            if (IsProfileUpdateEmpty(dto))
                return BadRequest(ApiResponse<object>.Fail("At least one field is required to update profile."));

            //Prevent meaningless full name like only spaces
            if (dto.FullName != null && string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest(ApiResponse<object>.Fail("Full name cannot be empty."));

            //Real-world DOB check
            if (dto.DateOfBirth.HasValue && dto.DateOfBirth.Value.Date > DateTime.Today)
                return BadRequest(ApiResponse<object>.Fail("Date of birth cannot be in the future."));

            //Reuse flexible phone validation
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && !IsValidPhoneNumber(dto.PhoneNumber))
                return BadRequest(ApiResponse<object>.Fail("Enter a valid phone number."));

            // Basic real-world image validation
            if (dto.ProfileImage != null)
            {
                if (!IsValidProfileImage(dto.ProfileImage))
                    return BadRequest(ApiResponse<object>.Fail("Only .jpg, .jpeg, .png, and .webp image files are allowed."));

                if (dto.ProfileImage.Length > 2 * 1024 * 1024)
                    return BadRequest(ApiResponse<object>.Fail("Profile image size must not exceed 2 MB."));
            }

            string? profileImagePath = null;

            // Save uploaded image locally and store relative path
            if (dto.ProfileImage != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile-images");

                // Create unique file name to avoid overwrite conflicts
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ProfileImage.FileName).ToLowerInvariant()}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(stream);
                }

                // Save relative path in DB, not absolute local machine path
                profileImagePath = $"/profile-images/{uniqueFileName}";
            }

            var result = await _authRepository.UpdateProfileAsync(userId, dto, profileImagePath);

            if (result == null)
                return BadRequest(ApiResponse<object>.Fail("Profile update failed"));

            return Ok(ApiResponse<object>.Ok(result, "Profile updated successfully"));
        }

        // ** REUSEABLE HELPER METHODS **

        // Checks practical password policy without overcomplicated regex
        private bool IsValidPassword(string password)
        {
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        // Allows common real-world phone formats like +91 98765 43210 or (079) 1234-5678
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Reject letters immediately
            if (phoneNumber.Any(char.IsLetter))
                return false;

            // Allow only realistic phone characters
            foreach (char ch in phoneNumber)
            {
                if (!(char.IsDigit(ch) || ch == ' ' || ch == '+' || ch == '-' || ch == '(' || ch == ')'))
                    return false;
            }

            // Count only digits for final sanity check
            string digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            return digitsOnly.Length >= 8 && digitsOnly.Length <= 15;
        }

        // Checks whether user actually sent any profile field to update
        private bool IsProfileUpdateEmpty(UpdateProfileDto dto)
        {
            return string.IsNullOrWhiteSpace(dto.FullName)
                && string.IsNullOrWhiteSpace(dto.PhoneNumber)
                && !dto.DateOfBirth.HasValue
                && string.IsNullOrWhiteSpace(dto.Address)
                && dto.ProfileImage == null;
        }

        // Validates common safe image formats for profile upload
        private bool IsValidProfileImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var allowedContentTypes = new[] { "image/jpg", "image/jpeg", "image/png", "image/webp" };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension) && allowedContentTypes.Contains(file.ContentType.ToLowerInvariant());
        }
    }
}