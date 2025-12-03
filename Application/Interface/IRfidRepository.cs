using RFID_Backend.Application.Models; 
namespace RFID_Backend.Application.Interface
{
    public interface IRfidRepository
    {
        Task<string> InsertEntryExitAsync(string uid, string location, string? customAction = null);
        Task<string?> GetFullNameByUidAsync(string uid);
        Task<List<UserDTO>> GetLogsAsync();
        Task<List<UserDTO>> GetFilteredLogsAsync(string? location, DateTime? fromDate, DateTime? toDate, string? uidOrName, string? action);
        Task<List<UserDTO>> GetUsersCurrentlyInsideAsync();
    }
}
