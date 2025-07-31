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
    public class UserDeliveryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserDeliveryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddDelivery([FromBody] Delivery delivery)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                return BadRequest("User with Id " + userId + " does not exist");
            }

            delivery.TotalPay = delivery.BasePay + delivery.TipPay;

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            var userShift = await _context.UserShifts
                .Where(us => us.UserId == userId)
                .Include(us => us.Shift)
                .FirstOrDefaultAsync(us => us.Shift.StartTime <= delivery.DeliveryTime 
                    && us.Shift.EndTime >= delivery.DeliveryTime
                    && us.Shift.App == delivery.App);

            var userDelivery = new UserDelivery
            {
                UserId = userId,
                DeliveryId = delivery.Id,
                DateAdded = DateTime.UtcNow
            };

            _context.UserDeliveries.Add(userDelivery);

            if (userShift != null) {
                var shiftDelivery = new ShiftDelivery {
                    UserId = userId,
                    ShiftId = userShift.Shift.Id,
                    DeliveryId = delivery.Id
                };

                _context.ShiftDeliveries.Add(shiftDelivery);
            }

            await _context.SaveChangesAsync();

            return Ok(new DeliveryDto
            {
                Id = delivery.Id,
                App = delivery.App,
                TotalPay = delivery.TotalPay,
                BasePay = delivery.BasePay,
                TipPay = delivery.TipPay,
                Mileage = delivery.Mileage,
                CustomerNeighborhood = delivery.CustomerNeighborhood,
                Restaurant = delivery.Restaurant,
                DeliveryTime = delivery.DeliveryTime,
                Notes = delivery.Notes,
            });
        }

        [HttpGet("my-deliveries")]
        public async Task<IActionResult> GetDeliveries()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userDeliveries = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .Include(ud => ud.Delivery)
                .Select(ud => new DeliveryDto
                {
                    Id = ud.Delivery.Id,
                    App = ud.Delivery.App,
                    DeliveryTime = ud.Delivery.DeliveryTime,
                    BasePay = ud.Delivery.BasePay,
                    TipPay = ud.Delivery.TipPay,
                    TotalPay = ud.Delivery.TotalPay,
                    Restaurant = ud.Delivery.Restaurant,
                    CustomerNeighborhood = ud.Delivery.CustomerNeighborhood,
                    Mileage = ud.Delivery.Mileage,
                    Notes = ud.Delivery.Notes
                })
                .ToListAsync();

            return Ok(userDeliveries);
        }

        [HttpGet("unassigned-deliveries")]
        public async Task<IActionResult> GetUnassignedDeliveries() {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var unassignedDeliveries = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId && !_context.ShiftDeliveries.Any(sd => sd.DeliveryId == ud.DeliveryId))
                .Include(ud => ud.Delivery)
                .Select(ud => new{
                    ud.Delivery.Id,
                    ud.Delivery.App,
                    ud.Delivery.DeliveryTime,
                    ud.Delivery.BasePay,
                    ud.Delivery.TipPay,
                    ud.Delivery.TotalPay,
                    ud.Delivery.Restaurant,
                    ud.Delivery.CustomerNeighborhood,
                    ud.Delivery.Notes,
                    ud.Delivery.Mileage
                })
                .ToListAsync();

            return Ok(unassignedDeliveries);
        }

        [HttpGet("filtered-deliveries")]
        public async Task<IActionResult> GetDeliveriesByApp([FromQuery] DeliveryApp? app,
            [FromQuery] double? basePay,
            [FromQuery] double? tipPay,
            [FromQuery] double? totalPay,
            [FromQuery] double? mileage,
            [FromQuery] string? restaurant,
            [FromQuery] string? customerNeighborhood)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userDeliveriesQuery = _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .Include(ud => ud.Delivery)
                .AsQueryable();

            if (app.HasValue)
            {
                userDeliveriesQuery = userDeliveriesQuery.Where(ud => ud.Delivery.App == app.Value);
            }

            if (basePay.HasValue)
            {
                userDeliveriesQuery = userDeliveriesQuery.Where(ud => ud.Delivery.BasePay >= basePay.Value);
            }

            if (tipPay.HasValue)
            {
                userDeliveriesQuery = userDeliveriesQuery.Where(ud => ud.Delivery.TipPay >= tipPay.Value);
            }

            if (totalPay.HasValue)
            {
                userDeliveriesQuery = userDeliveriesQuery.Where(ud => ud.Delivery.TotalPay >= totalPay.Value);
            }

            if (mileage.HasValue)
            {
                userDeliveriesQuery = userDeliveriesQuery.Where(ud => ud.Delivery.Mileage >= mileage.Value);
            }

            if (!string.IsNullOrEmpty(restaurant))
            {
                userDeliveriesQuery = userDeliveriesQuery.Where(ud => ud.Delivery.Restaurant.ToLower().Contains(restaurant.ToLower()));
            }

            if (!string.IsNullOrEmpty(customerNeighborhood))
            {
                userDeliveriesQuery = userDeliveriesQuery
                    .Where(ud => ud.Delivery.CustomerNeighborhood.ToLower().Contains(customerNeighborhood.ToLower()));
            }

            var userDeliveries = await userDeliveriesQuery
                .Select(ud => new DeliveryDto
                {
                    Id = ud.Delivery.Id,
                    App = ud.Delivery.App,
                    DeliveryTime = ud.Delivery.DeliveryTime,
                    BasePay = ud.Delivery.BasePay,
                    TipPay = ud.Delivery.TipPay,
                    TotalPay = ud.Delivery.TotalPay,
                    Restaurant = ud.Delivery.Restaurant,
                    CustomerNeighborhood = ud.Delivery.CustomerNeighborhood,
                    Mileage = ud.Delivery.Mileage,
                    Notes = ud.Delivery.Notes
                })
                .ToListAsync();

            return Ok(userDeliveries);
        }

        [HttpGet("delivery-neighborhoods")]
        public async Task<IActionResult> GetUserDeliveryNeighborhoods()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userNeighborhoods = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .Select(ud => ud.Delivery.CustomerNeighborhood)
                .Where(n => n != null)
                .Distinct()
                .ToListAsync();

            return Ok(userNeighborhoods);
        }

        [HttpGet("delivery-apps")]
        public async Task<IActionResult> GetUserDeliveryApps()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userApps = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .Select(ud => ud.Delivery.App)
                .Distinct()
                .ToListAsync();

            return Ok(userApps);
        }

        [HttpGet("{deliveryId:int}")]
        public async Task<IActionResult> GetDeliveryById(int deliveryId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userDelivery = await _context.UserDeliveries
                .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DeliveryId == deliveryId);

            if (userDelivery == null)
            {
                return NotFound("Delivery not found");
            }

            return Ok(userDelivery);
        }

        [HttpDelete("{deliveryId}")]
        public async Task<IActionResult> DeleteDelivery(int deliveryId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userDelivery = await _context.UserDeliveries
                .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DeliveryId == deliveryId);

            if (userDelivery == null)
            {
                return NotFound("Delivery not found");
            }
            
            var shiftDelivery = await _context.ShiftDeliveries
                .FirstOrDefaultAsync(sd => sd.UserId == userId && sd.DeliveryId == deliveryId);


            // Remove userDelivery
            _context.UserDeliveries.Remove(userDelivery);
            await _context.SaveChangesAsync();


            // If delivery is associated with shift, remove shiftDelivery
            if (shiftDelivery != null) {
                _context.ShiftDeliveries.Remove(shiftDelivery);
                await _context.SaveChangesAsync();
            }

            var delivery = await _context.Deliveries.FindAsync(deliveryId);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
                await _context.SaveChangesAsync();
            }

            return Ok("Delivery removed");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDelivery([FromBody] Delivery delivery) {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var existingDelivery = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId && ud.Delivery.Id == delivery.Id)
                .Include(ud => ud.Delivery)
                .FirstOrDefaultAsync();

            if (existingDelivery == null) {
                return BadRequest("Delivery does not exist");
            }

            var targetDelivery = existingDelivery.Delivery;

            targetDelivery.App = delivery.App;
            targetDelivery.TotalPay = delivery.TotalPay;
            targetDelivery.TipPay = delivery.TipPay;
            targetDelivery.BasePay = delivery.BasePay;
            targetDelivery.Mileage = delivery.Mileage;
            targetDelivery.Restaurant = delivery.Restaurant;
            targetDelivery.CustomerNeighborhood = delivery.CustomerNeighborhood;
            targetDelivery.DeliveryTime = delivery.DeliveryTime;
            targetDelivery.Notes = delivery.Notes;

            _context.Deliveries.Update(targetDelivery);
            await _context.SaveChangesAsync();

            var responseDelivery = new {
                targetDelivery.Id,
                targetDelivery.App,
                targetDelivery.TotalPay,
                targetDelivery.TipPay,
                targetDelivery.BasePay,
                targetDelivery.Mileage,
                targetDelivery.Restaurant,
                targetDelivery.CustomerNeighborhood,
                targetDelivery.DeliveryTime,
                targetDelivery.Notes
            };

            return Ok(responseDelivery);
        }
    }
}