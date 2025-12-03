using Dapper;
using RFID_Backend.Application.Interface;
using RFID_Backend.Application.Models;
using System.Data.SqlClient;

namespace RFID_Backend.Infrastructure.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> RegisterUserAsync(RegisterRequestDTO req)
        {
            using var conn = new SqlConnection(_connectionString);

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM UserLogin WHERE Email=@Email", new { req.Email });

            if (exists > 0) return 0;

            string insertUser = @"
                INSERT INTO Users (UserId, FullName, Address, PhoneNumber, Gender, DateOfBirth, DesignationId, CreatedDate, IsActive)
                VALUES (@UserId, @FullName, @Address, @PhoneNumber, @Gender, @DateOfBirth, @DesignationId, GETDATE(), 1)";
            await conn.ExecuteAsync(insertUser, req);

            string insertLogin = @"
                INSERT INTO UserLogin (UserId, Email, PasswordHash, CreatedDate, IsActive)
                VALUES (@UserId, @Email, @Password, GETDATE(), 1)";
            await conn.ExecuteAsync(insertLogin, new { req.UserId, req.Email, req.Password });

            return 1;
        }

        public async Task<LoginResponseDTO> ValidateUserAsync(string email)
        {
            using var conn = new SqlConnection(_connectionString);

            string sql = @"
                SELECT u.UserId, u.FullName, d.DesignationName AS Designation, l.PasswordHash
                FROM UserLogin l
                INNER JOIN Users u ON l.UserId = u.UserId
                INNER JOIN Designation d ON u.DesignationId = d.DesignationId
                WHERE l.Email = @Email AND l.IsActive = 1 AND u.IsActive = 1";

            return await conn.QueryFirstOrDefaultAsync<LoginResponseDTO>(sql, new { Email = email });
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);

            string sql = @"
                SELECT u.UserId, u.FullName, l.Email, l.PasswordHash
                FROM Users u
                INNER JOIN UserLogin l ON u.UserId = l.UserId
                WHERE u.UserId = @UserId";

            return await conn.QueryFirstOrDefaultAsync<UserDTO>(sql, new { UserId = userId });
        }

        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            using var conn = new SqlConnection(_connectionString);

            string sql = "UPDATE UserLogin SET PasswordHash=@PasswordHash WHERE UserId=@UserId";

            await conn.ExecuteAsync(sql, new { UserId = userId, PasswordHash = newPassword });
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            using var conn = new SqlConnection(_connectionString);

            string sql = @"
                SELECT u.UserId, u.FullName, l.Email, l.PasswordHash
                FROM Users u
                INNER JOIN UserLogin l ON u.UserId = l.UserId
                WHERE l.Email = @Email";

            return await conn.QueryFirstOrDefaultAsync<UserDTO>(sql, new { Email = email });
        }

        public async Task SaveOtpAsync(int userId, string otp)
        {
            using var conn = new SqlConnection(_connectionString);

            string sql = @"
                INSERT INTO UserOtp (UserId, OtpCode, OtpExpiry)
                VALUES (@UserId, @OtpCode, DATEADD(MINUTE, 5, GETDATE()))";

            await conn.ExecuteAsync(sql, new { UserId = userId, OtpCode = otp });
        }

        public async Task<UserOtpDTO> GetLatestOtpAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);

            string sql = @"
                SELECT TOP 1 * FROM UserOtp
                WHERE UserId=@UserId AND IsUsed=0
                ORDER BY CreatedAt DESC";

            return await conn.QueryFirstOrDefaultAsync<UserOtpDTO>(sql, new { UserId = userId });
        }

        public async Task MarkOtpUsedAsync(int otpId)
        {
            using var conn = new SqlConnection(_connectionString);

            string sql = "UPDATE UserOtp SET IsUsed=1 WHERE OtpId=@OtpId";

            await conn.ExecuteAsync(sql, new { OtpId = otpId });
        }
    }
}
