using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFID_Backend.Application.Interface;

namespace RFID_Backend.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RfidController : ControllerBase
    {
        private readonly IRfidRepository _repo;

        public RfidController(IRfidRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("scan")]
        public async Task<IActionResult> Scan(string uid, string location)
        {
            return Ok(await _repo.InsertEntryExitAsync(uid, location));
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            return Ok(await _repo.GetLogsAsync());
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFiltered(string? location, DateTime? from, DateTime? to, string? uidOrName, string? action)
        {
            return Ok(await _repo.GetFilteredLogsAsync(location, from, to, uidOrName, action));
        }

        [HttpGet("inside")]
        public async Task<IActionResult> GetUsersInside()
        {
            return Ok(await _repo.GetUsersCurrentlyInsideAsync());
        }
    }
}
