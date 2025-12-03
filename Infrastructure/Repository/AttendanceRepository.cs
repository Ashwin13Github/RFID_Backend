using RFID_Backend.Application.Interface;
using RFID_Backend.Application.Models;
using System.Data.SqlClient;
using Dapper;
namespace RFID_Backend.Infrastructure.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly string _connectionString;

        public AttendanceRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<AttendanceModel>> GetAttendanceAsync(string? uid, DateTime? date)
        {
            using var conn = new SqlConnection(_connectionString);

            var result = await conn.QueryAsync<AttendanceModel>(
                "sp_GetAttendance",
                new { Uid = uid, Date = date },
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}
