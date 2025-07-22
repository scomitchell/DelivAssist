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
                ShiftId = shift.Id,
                DateAdded = DateTime.UtcNow
            };

            _context.UserShifts.Add(userShift);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                startTime = shift.StartTime,
                endTime = shift.EndTime,
                app = shift.App
            });
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
                    us.Shift.EndTime,
                    us.Shift.App
                })
                .ToListAsync();

            return Ok(userShifts);
        }

        [HttpGet("apps")]
        public async Task<IActionResult> GetUserShiftApps()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserShifts
                .Where(us => us.UserId == userId)
                .Select(us => us.Shift.App)
                .Distinct()
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("filtered-shifts")]
        public async Task<IActionResult> GetFilteredShifts([FromQuery] DateTime? startTime,
            [FromQuery] DateTime? endTime, 
            [FromQuery] DeliveryApp? app)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userShiftsQuery = _context.UserShifts
                .Where(us => us.UserId == userId)
                .Include(us => us.Shift)
                .AsQueryable();

            if (startTime.HasValue)
            {
                userShiftsQuery = userShiftsQuery.Where(us => us.Shift.StartTime >= startTime);
            }

            if (endTime.HasValue)
            {
                userShiftsQuery = userShiftsQuery.Where(us => us.Shift.EndTime <= endTime);
            }

            if (app.HasValue)
            {
                userShiftsQuery = userShiftsQuery.Where(us => us.Shift.App == app);
            }

            var userShifts = await userShiftsQuery
                .Select(us => new
                {
                    us.Shift.Id,
                    us.Shift.StartTime,
                    us.Shift.EndTime,
                    us.Shift.App
                })
                .ToListAsync();

            return Ok(userShifts);
        }

        [HttpGet("{shiftId:int}")]
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