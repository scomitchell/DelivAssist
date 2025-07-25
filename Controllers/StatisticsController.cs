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

        [HttpGet("deliveries/highest-paying-restaurant")]
        public async Task<IActionResult> GetHighestPayingRestaurant()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .GroupBy(ud => ud.Delivery.Restaurant)
                .Select(g => new
                {
                    Restaurant = g.Key,
                    AvgTotalPay = g.Average(ud => ud.Delivery.TotalPay)
                })
                .OrderByDescending(x => x.AvgTotalPay)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound("No deliveries found for user");
            }

            return Ok(new
            {
                restaurant = result.Restaurant,
                avgTotalPay = result.AvgTotalPay
            });
        }

        [HttpGet("deliveries/highest-paying-base-app")]
        public async Task<IActionResult> GetHighestPayingBaseApp()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .GroupBy(ud => ud.Delivery.App)
                .Select(g => new
                {
                    App = g.Key,
                    AverageBase = g.Average(ud => ud.Delivery.BasePay)
                })
                .OrderByDescending(x => x.AverageBase)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound("No deliveries found");
            }

            return Ok(new
            {
                app = result.App,
                avgBase = result.AverageBase
            });
        }

        [HttpGet("deliveries/highest-paying-tip-app")]
        public async Task<IActionResult> GetHighestPayingTipApp()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .GroupBy(ud => ud.Delivery.App)
                .Select(g => new
                {
                    App = g.Key,
                    AverageTip = g.Average(ud => ud.Delivery.TipPay)
                })
                .OrderByDescending(x => x.AverageTip)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound("No deliveries found");
            }

            return Ok(new
            {
                app = result.App,
                avgTip = result.AverageTip
            });
        }

        [HttpGet("deliveries/dollar-per-mile")]
        public async Task<IActionResult> GetDollarPerMile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var miles = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .SumAsync(ud => ud.Delivery.Mileage);

            var totalPay = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .SumAsync(ud => ud.Delivery.TotalPay);

            if (miles == 0 || totalPay == 0)
            {
                return Ok(0);
            }

            var result = totalPay / miles;

            return Ok(result);
        }

        [HttpGet("expenses/average-monthly-spending")]
        public async Task<IActionResult> GetAverageMonthlySpending() {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .GroupBy(ue => new { ue.Expense.Date.Year, ue.Expense.Date.Month })
                .Select(g => g.Sum(ue => ue.Expense.Amount))
                .Average();

            if (result == null) {
                return NotFound("No expenses found");
            }

            return Ok(result);
        }

        [HttpGet("expenses/average-spending-by-type")]
        public async Task<IActionResult> GetAverageSpendingByType() {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExpenses = await _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .Select(ue => new {
                    ue.Expense.Type,
                    ue.Expense.Amount,
                    Month = ue.Expense.Date.Month,
                    Year = ue.Expense.Date.Year
                })
                .ToListAsync();

            var totalMonths = userExpenses
                .Select(e => new {e.Year, e.Month})
                .Distinct()
                .Count();

            var result = userExpenses
                .GroupBy(e => e.Type)
                .Select(g => new {
                    Type = g.Key,
                    AvgExpense = totalMonths > 0 ? g.Sum(x => x.Amount) / totalMonths : 0
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("shifts/average-shift-length")]
        public async Task<IActionResult> getAverageShiftLength() {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var durations = await _context.UserShifts
                .Where(us => us.UserId == userId)
                .Select(us => us.Shift.EndTime - us.Shift.StartTime)
                .ToListAsync();

            if (durations.Count == 0)
                return Ok(0);

            var averageMinutes = durations.Average(d => d.TotalMinutes);

            return Ok(averageMinutes);
        }

        [HttpGet("shifts/app-with-most-shifts")]
        public async Task<IActionResult> getAppWithMostShifts() {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserShifts
                .Where(us => us.UserId == userId)
                .GroupBy(us => us.Shift.App)
                .Select(g => new {
                    App = g.Key,
                    ShiftCount = g.Count()
                })
                .OrderByDescending(g => g.ShiftCount)
                .FirstOrDefaultAsync();

            if (result == null) {
                return Ok(null);
            }

            return Ok(result.App);
        }
    }
}