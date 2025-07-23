using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DelivAssist.Data;
using DelivAssist.Models;

namespace DelivAssist.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShiftDeliveryController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public ShiftDeliveryController(ApplicationDbContext context) {
            _context = context;
        }

        [HttpPost("{shiftId:int}")]
        public async Task<IActionResult> AddShiftDelivery([FromBody] Delivery delivery, int shiftId) {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists) {
                return BadRequest("User does not exist");
            }

            var shiftDelivery = new ShiftDelivery {
                ShiftId = shiftId,
                UserId = userId,
                DeliveryId = delivery.Id
            };

            _context.ShiftDeliveries.Add(shiftDelivery);
            await _context.SaveChangesAsync();

            return Ok("ShiftDelivery added");
        }

        [HttpGet("{shiftId:int}")]
        public async Task<IActionResult> GetDeliveriesForShift(int shiftId) {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var shiftDeliveries = await _context.ShiftDeliveries
                .Where(sd => sd.UserId == userId && sd.ShiftId == shiftId)
                .Include(sd => sd.Delivery)
                .Select(sd => new {
                    sd.Delivery.Id,
                    sd.Delivery.App,
                    sd.Delivery.DeliveryTime,
                    sd.Delivery.BasePay,
                    sd.Delivery.TipPay,
                    sd.Delivery.TotalPay,
                    sd.Delivery.Mileage,
                    sd.Delivery.Restaurant,
                    sd.Delivery.CustomerNeighborhood,
                    sd.Delivery.Notes
                })
                .ToListAsync();

            return Ok(shiftDeliveries);
        }
    }
}