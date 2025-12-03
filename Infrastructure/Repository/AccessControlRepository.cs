using Dapper;
using RFID_Backend.Application.Interface;
using System.Data.SqlClient;

namespace RFID_Backend.Infrastructure.Repository
{
    public class AccessControlRepository : IAccessControlRepository
    {
        private readonly string _connectionString;

        public AccessControlRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<(int UserId, int DesignationId, string FullName)?> GetUserInfoAsync(string uid)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<(int, int, string)?>(@"
                SELECT u.UserId, u.DesignationId, u.FullName
                FROM Users u
                JOIN UserUID uu ON uu.UserId = u.UserId
                WHERE uu.UID = @Uid", new { Uid = uid });
        }

        public async Task<bool> CanAccessLocationAsync(int designationId, string location)
        {
            using var conn = new SqlConnection(_connectionString);

            var count = await conn.ExecuteScalarAsync<int>(@"
                SELECT COUNT(*)
                FROM AccessPermissions ap
                JOIN AccessPoints a ON ap.AccessPointId = a.AccessPointId
                WHERE ap.DesignationId = @DesignationId
                AND a.LocationName = @Location
                AND ap.IsActive = 1",
                new { DesignationId = designationId, Location = location });

            return count > 0;
        }

        public async Task<(string Designation, string ClassOrDept)?> GetDesignationAndClassAsync(string uid)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<(string, string)?>(@"
                SELECT TOP 1 
                    ISNULL(Designation, '') AS Designation,
                    ISNULL(ClassOrDept, '') AS ClassOrDept
                FROM RFIDLogs
                WHERE Uid = @Uid
                ORDER BY Timestamp DESC",
                new { Uid = uid });
        }
    }
}