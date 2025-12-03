namespace RFID_Backend.Application.Models
{
    public class UserOtpDTO
    {
        public int OtpId { get; set; }
        public int UserId { get; set; }
        public string OtpCode { get; set; }
        public DateTime OtpExpiry { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
