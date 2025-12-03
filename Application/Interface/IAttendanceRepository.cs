using RFID_Backend.Application.Models;

namespace RFID_Backend.Application.Interface
{
    public interface IAttendanceRepository
    {
        Task<List<AttendanceModel>> GetAttendanceAsync(string? uid = null, DateTime? date = null);
    }
}
