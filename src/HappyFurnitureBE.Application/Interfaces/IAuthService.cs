using HappyFurnitureBE.Application.DTOs.Auth;

namespace HappyFurnitureBE.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<string> GenerateJwtTokenAsync(UserDto user);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<bool> ValidatePasswordAsync(string email, string password);
}