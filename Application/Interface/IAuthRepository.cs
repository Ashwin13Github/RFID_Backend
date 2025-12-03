using RFID_Backend.Application.Models;

namespace RFID_Backend.Application.Interface
{
    public interface IAuthRepository
    {
        Task<int> RegisterUserAsync(RegisterRequestDTO req);
        Task<LoginResponseDTO> ValidateUserAsync(string email);
        Task<UserDTO> GetUserByIdAsync(int userId);
        Task UpdatePasswordAsync(int userId, string newPassword);
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task SaveOtpAsync(int userId, string otp);
        Task<UserOtpDTO> GetLatestOtpAsync(int userId);
        Task MarkOtpUsedAsync(int otpId);
    }
}
