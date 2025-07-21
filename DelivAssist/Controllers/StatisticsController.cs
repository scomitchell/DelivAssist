using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DelivAssist.Data;
using DelivAssist.Models;

namespace DelivAssist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("deliveries/avg-delivery-pay")]
        public async Task<IActionResult> GetAvgDeliveryPay()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get all the total pays for deliveries of this user
            var totalPays = await (
                from ud in _context.UserDeliveries
                join d in _context.Deliveries on ud.DeliveryId equals d.Id
                where ud.UserId == userId
                select d.TotalPay
            ).ToListAsync();

            // If no deliveries, average is zero (or handle differently)
            if (totalPays.Count == 0)
            {
                return Ok(0);
            }

            // Compute average in memory
            var avgPay = totalPays.Average();

            return Ok(avgPay);
        }

        [HttpGet("deliveries/average-base-pay")]
        public async Task<IActionResult> GetAvgBasePay()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var totalBases = await (
                from ud in _context.UserDeliveries
                join d in _context.Deliveries on ud.DeliveryId equals d.Id
                where ud.UserId == userId
                select d.BasePay
            ).ToListAsync();

            if (totalBases.Count == 0)
            {
                return Ok(0);
            }

            var avgBase = totalBases.Average();

            return Ok(avgBase);
        }

        [HttpGet("deliveries/average-tip")]
        public async Task<IActionResult> GetAvgTipPay()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var totalTips = await (
                from ud in _context.UserDeliveries
                join d in _context.Deliveries on ud.DeliveryId equals d.Id
                where ud.UserId == userId
                select d.TipPay
            ).ToListAsync();

            if (totalTips.Count == 0)
            {
                return Ok(0);
            }

            var avgTip = totalTips.Average();

            return Ok(avgTip);
        }

        [HttpGet("deliveries/highest-paying-neighborhood")]
        public async Task<IActionResult> GetHighestPayingNeighborhood()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .GroupBy(ud => ud.Delivery.CustomerNeighborhood)
                .Select(g => new
                {
                    Neighborhood = g.Key,
                    AvgTipPay = g.Average(ud => ud.Delivery.TipPay)
                })
                .OrderByDescending(x => x.AvgTipPay)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound("No deliveries found for user");
            }

            return Ok(new
            {
                neighborhood = result.Neighborhood,
                averageTipPay = result.AvgTipPay
            });
        }
    }
}