using SmartStay.API.DTOs.Auth;
using SmartStay.API.Models;

namespace SmartStay.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ProfileResponseDto?> GetProfileAsync(int userId);
        Task<ProfileResponseDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto, string? profileImagePath);
    }
}