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
        public async Task<IActionResult> AddShiftDelivery(int shiftId, [FromQuery] int deliveryId) {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists) {
                return BadRequest("User does not exist");
            }

            var shift = await _context.Shifts.FindAsync(shiftId);

            if (shift == null) {
                return BadRequest("Shift does not exist");
            }

            var delivery = await _context.Deliveries.FindAsync(deliveryId);

            if (delivery == null) {
                return BadRequest("Delivery does not exist");
            }

            if (delivery.DeliveryTime < shift.StartTime || delivery.DeliveryTime > shift.EndTime) {
                return BadRequest("Delivery does not fit within shift time");
            }

            if (delivery.App != shift.App) {
                return BadRequest("Delivery app does not match shift app");
            }

            var shiftDelivery = new ShiftDelivery {
                ShiftId = shiftId,
                UserId = userId,
                DeliveryId = deliveryId
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
                .Select(sd => new DeliveryDto {
                    Id = sd.Delivery.Id,
                    App = sd.Delivery.App,
                    DeliveryTime = sd.Delivery.DeliveryTime,
                    BasePay = sd.Delivery.BasePay,
                    TipPay = sd.Delivery.TipPay,
                    TotalPay = sd.Delivery.TotalPay,
                    Mileage = sd.Delivery.Mileage,
                    Restaurant = sd.Delivery.Restaurant,
                    CustomerNeighborhood = sd.Delivery.CustomerNeighborhood,
                    Notes = sd.Delivery.Notes
                })
                .ToListAsync();

            return Ok(shiftDeliveries);
        }
    }
}