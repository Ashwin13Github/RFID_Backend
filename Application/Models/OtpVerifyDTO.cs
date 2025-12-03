namespace RFID_Backend.Application.Models
{
    public class OtpVerifyDTO
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
