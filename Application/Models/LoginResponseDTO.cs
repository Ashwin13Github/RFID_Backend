namespace RFID_Backend.Application.Models
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Token { get; set; }
        public string PasswordHash { get; set; }
    }
}
