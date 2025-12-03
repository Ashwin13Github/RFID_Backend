
using Microsoft.AspNetCore.Mvc;
using RFID_Backend.Application.Interface;

namespace RFID_Backend.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _repo;

        public AttendanceController(IAttendanceRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendance(string? uid, DateTime? date)
        {
            return Ok(await _repo.GetAttendanceAsync(uid, date));
        }
    }
}