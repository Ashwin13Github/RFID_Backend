namespace RFID_Backend.Application.Interface
{
    public interface IAccessControlRepository
    {
        Task<(int UserId, int DesignationId, string FullName)?> GetUserInfoAsync(string uid);
        Task<bool> CanAccessLocationAsync(int designationId, string location);
        Task<(string Designation, string ClassOrDept)?> GetDesignationAndClassAsync(string uid);
    }
}
