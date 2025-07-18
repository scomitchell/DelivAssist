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

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            var userDelivery = new UserDelivery
            {
                UserId = userId,
                DeliveryId = delivery.Id
            };

            _context.UserDeliveries.Add(userDelivery);
            await _context.SaveChangesAsync();

            return Ok("Delivery Added");
        }

        [HttpGet("my-deliveries")]
        public async Task<IActionResult> GetDeliveries()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userDeliveries = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .Include(ud => ud.Delivery)
                .Select(ud => new
                {
                    ud.Delivery.Id,
                    ud.Delivery.App,
                    ud.Delivery.DeliveryTime,
                    ud.Delivery.BasePay,
                    ud.Delivery.TipPay,
                    ud.Delivery.TotalPay,
                    ud.Delivery.Restaurant,
                    ud.Delivery.RestaurantStreetAddress,
                    ud.Delivery.CustomerNeighborhood,
                    ud.Delivery.Notes
                })
                .ToListAsync();

            return Ok(userDeliveries);
        }

        [HttpGet("filtered-deliveries")]
        public async Task<IActionResult> GetDeliveriesByApp([FromQuery] DeliveryApp? app,
            [FromQuery] double? basePay,
            [FromQuery] double? tipPay,
            [FromQuery] double? totalPay,
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
                .Select(ud => new
                {
                    ud.Delivery.Id,
                    ud.Delivery.App,
                    ud.Delivery.DeliveryTime,
                    ud.Delivery.BasePay,
                    ud.Delivery.TipPay,
                    ud.Delivery.TotalPay,
                    ud.Delivery.Restaurant,
                    ud.Delivery.RestaurantStreetAddress,
                    ud.Delivery.CustomerNeighborhood,
                    ud.Delivery.Notes
                })
                .ToListAsync();

            return Ok(userDeliveries);
        }

        [HttpGet("{deliveryId}")]
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

            _context.UserDeliveries.Remove(userDelivery);
            await _context.SaveChangesAsync();

            return Ok("Delivery removed");
        }
    }
}