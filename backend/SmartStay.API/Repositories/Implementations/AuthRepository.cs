using Dapper;
using Microsoft.Data.SqlClient;
using SmartStay.API.DTOs.Auth;
using SmartStay.API.Helpers;
using SmartStay.API.Models;
using SmartStay.API.Repositories.Interfaces;
using System.Data;

namespace SmartStay.API.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;
        private readonly JwtHelper _jwtHelper;

        public AuthRepository(IConfiguration config, JwtHelper jwtHelper)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _jwtHelper = jwtHelper;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<User?> RegisterAsync(RegisterRequestDto dto)
        {
            using var conn = Connection;

            var hashedPassword = PasswordHelper.HashPassword(dto.Password);

            var result = await conn.QueryFirstOrDefaultAsync<User>(
                "sp_User_Register",
                new
                {
                    dto.FullName,
                    dto.Email,
                    PasswordHash = hashedPassword,
                    dto.PhoneNumber,
                    Role = "Customer"
                },
                commandType: CommandType.StoredProcedure
            );

            // If SP returns only Success/Message on duplicate email,
            // Dapper maps default values, so treat UserID 0 as failure
            if (result == null || result.UserID == 0)
                return null;

            return result;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            using var conn = Connection;

            var user = await conn.QueryFirstOrDefaultAsync<User>(
                "sp_User_Login",
                new { dto.Email },
                commandType: CommandType.StoredProcedure
            );

            if (user == null)
                return null;

            bool isValidPassword = PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash);

            if (!isValidPassword)
                return null;

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Role = user.Role,
                Token = token
            };
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            using var conn = Connection;

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_User_ForgotPassword",
                new { dto.Email },
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.Success == 0)
                return false;

            //OTP should be sent by email/SMS here
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            using var conn = Connection;

            var hashedPassword = PasswordHelper.HashPassword(dto.NewPassword);

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_User_ResetPassword",
                new
                {
                    dto.Email,
                    dto.OtpCode,
                    NewPasswordHash = hashedPassword
                },
                commandType: CommandType.StoredProcedure
            );

            if (result == null || result.Success == 0)
                return false;

            return true;
        }

        public async Task<ProfileResponseDto?> GetProfileAsync(int userId)
        {
            using var conn = Connection;

            return await conn.QueryFirstOrDefaultAsync<ProfileResponseDto>(
                "sp_User_GetProfile",
                new { UserID = userId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<ProfileResponseDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            using var conn = Connection;

            await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_User_UpdateProfile",
                new
                {
                    UserID = userId,
                    dto.FullName,
                    dto.PhoneNumber,
                    dto.DateOfBirth,
                    dto.Address,
                    ProfileImage = dto.ProfileImage?.FileName
                },
                commandType: CommandType.StoredProcedure
            );

            return await GetProfileAsync(userId);
        }
    }
}