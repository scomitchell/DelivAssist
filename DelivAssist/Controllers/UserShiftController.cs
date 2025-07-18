using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using DelivAssist.Data;
using DelivAssist.Models;

namespace DelivAssist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserShiftController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserShiftController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddShift([FromBody] Shift shift)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            var userShift = new UserShift
            {
                UserId = userId,
                ShiftId = shift.Id
            };

            _context.UserShifts.Add(userShift);
            await _context.SaveChangesAsync();

            return Ok("Shift Added");
        }

        [HttpGet("my-shifts")]
        public async Task<IActionResult> GetShifts()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userShifts = await _context.UserShifts
                .Where(us => us.UserId == userId)
                .Include(us => us.Shift)
                .Select(us => new
                {
                    us.Shift.Id,
                    us.Shift.StartTime,
                    us.Shift.EndTime
                })
                .ToListAsync();

            return Ok(userShifts);
        }

        [HttpGet("{shiftId}")]
        public async Task<IActionResult> GetShiftById(int shiftId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userShift = await _context.UserShifts
                .FirstOrDefaultAsync(us => us.UserId == userId && us.ShiftId == shiftId);

            if (userShift == null)
            {
                return NotFound("Shift not found");
            }

            return Ok(userShift);
        }

        [HttpDelete("{shiftId}")]
        public async Task<IActionResult> DeleteShift(int shiftId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userShift = await _context.UserShifts
                .FirstOrDefaultAsync(us => us.UserId == userId && us.ShiftId == shiftId);

            if (userShift == null)
            {
                return NotFound("Shift not found");
            }

            _context.UserShifts.Remove(userShift);
            await _context.SaveChangesAsync();

            return Ok("Shift Deleted");
        }
    }
}