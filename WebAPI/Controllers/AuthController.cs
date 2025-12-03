using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RFID_Backend.Application.Interface;
using RFID_Backend.Application.Models;
using RFID_Backend.Infrastructure.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RFID_Backend.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authDal;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailservice;
        public AuthController(IAuthRepository authDal, IConfiguration config, IEmailService emailservice)
        {
            _authDal = authDal;
            _config = config;
            _emailservice = emailservice;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password required.");

            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var result = await _authDal.RegisterUserAsync(request);
            if (result == 0) return BadRequest("Email already exists.");

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var user = await _authDal.ValidateUserAsync(request.Email);
            if (user == null) return Unauthorized("Invalid credentials");

            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!validPassword) return Unauthorized("Invalid credentials");

            // JWT Token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Designation ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return Ok(new
            {
                user.UserId,
                user.FullName,
                user.Designation,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _authDal.GetUserByIdAsync(int.Parse(userId));
            if (user == null) return NotFound("User not found");

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
                return BadRequest("Current password incorrect");

            var newHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _authDal.UpdatePasswordAsync(user.UserId, newHash);

            return Ok("Password changed successfully");
        }

        [HttpPost("RequestOtp")]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequestDTO model)
        {
            var user = await _authDal.GetUserByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User not found");

            string otp = new Random().Next(100000, 999999).ToString();

            await _authDal.SaveOtpAsync(user.UserId, otp);

            await _emailservice.SendEmailAsync(
                user.Email,
                "Your OTP Code",
                $"<h2>Your OTP is: <b>{otp}</b></h2><p>Valid for 5 minutes.</p>"
            );

            return Ok("OTP sent successfully");
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDTO model)
        {
            var user = await _authDal.GetUserByEmailAsync(model.Email);
            if (user == null) return NotFound("User not found");

            var otpData = await _authDal.GetLatestOtpAsync(user.UserId);
            if (otpData == null) return BadRequest("OTP not generated");

            if (otpData.OtpCode != model.Otp)
                return BadRequest("Invalid OTP");

            if (otpData.OtpExpiry < DateTime.UtcNow)
                return BadRequest("OTP expired");

            await _authDal.MarkOtpUsedAsync(otpData.OtpId);

            var newHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _authDal.UpdatePasswordAsync(user.UserId, newHash);

            return Ok("Password reset successfully");
        }
    }
}
