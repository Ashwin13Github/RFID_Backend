using RFID_Backend.Application.Interface;
using System.Data.SqlClient;
using Dapper;
using RFID_Backend.Application.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic; // Add this for List<T>

namespace RFID_Backend.Infrastructure.Repository
{
    public class RfidRepository : IRfidRepository
    {
        private readonly string _connectionString;

        public RfidRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<string> InsertEntryExitAsync(string uid, string location, string? customAction = null)
        {
            using var conn = new SqlConnection(_connectionString);

            var result = await conn.QueryFirstOrDefaultAsync<string>(
                "sp_InsertEntryExit",
                new { Uid = uid, Location = location, CustomAction = customAction },
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result ?? "Unknown";
        }

        public async Task<string?> GetFullNameByUidAsync(string uid)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.ExecuteScalarAsync<string?>(@"
                SELECT TOP 1 u.FullName
                FROM Users u
                INNER JOIN UserUID uu ON uu.UserId = u.UserId
                WHERE uu.UID = @Uid", new { Uid = uid });
        }

        public async Task<List<UserDTO>> GetLogsAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            var logs = await conn.QueryAsync<UserDTO>(@"
                SELECT r.Uid, u.FullName, r.Location, r.Action, r.Timestamp
                FROM RFIDLogs r
                INNER JOIN UserUID uu ON r.Uid = uu.UID
                INNER JOIN Users u ON uu.UserId = u.UserId
                ORDER BY r.Timestamp ASC");

            return logs.ToList();
        }

        public async Task<List<UserDTO>> GetFilteredLogsAsync(
            string? location, DateTime? fromDate, DateTime? toDate, string? uidOrName, string? action)
        {
            using var conn = new SqlConnection(_connectionString);

            var logs = await conn.QueryAsync<UserDTO>(
                "sp_GetFilteredLogs",
                new { location, fromDate, toDate, uidOrName, action },
                commandType: System.Data.CommandType.StoredProcedure
            );

            return logs.ToList();
        }

        public async Task<List<UserDTO>> GetUsersCurrentlyInsideAsync()
        {
            using var conn = new SqlConnection(_connectionString);

            var result = await conn.QueryAsync<UserDTO>(
                "sp_GetUsersInside",
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}

