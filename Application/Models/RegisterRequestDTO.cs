namespace RFID_Backend.Application.Models
{
    public class RegisterRequestDTO
    {
        public int UserId { get; set; }           // Manual entry
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int DesignationId { get; set; }

        // Login info
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
