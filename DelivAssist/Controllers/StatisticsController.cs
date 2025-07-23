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

        [HttpGet("expenses/highest-type")]
        public async Task<IActionResult> GetHighestExpenseType() {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .GroupBy(ue => ue.Expense.Type)
                .Select(g => new {
                    Type = g.Key,
                    AvgAmount = g.Average(ue => ue.Expense.Amount)
                })
                .OrderByDescending(x => x.AvgAmount)
                .FirstOrDefaultAsync();

            if (result == null) {
                return NotFound("No expenses found");
            }

            return Ok(new {
                type = result.Type,
                avgAmount = result.AvgAmount
            });
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

            var result = await _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .GroupBy(ue => new {ue.Expense.Date.Year, ue.Expense.Date.Month, ue.Expense.Type})
                .Select(g => new {
                    Type = g.Key.Type,
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    MonthlyTotal = g.Sum(x => x.Expense.Amount)
                })
                .GroupBy(x => x.Type)
                .Select(g => new {
                    Type = g.Key,
                    AvgExpense = g.Average(x => x.MonthlyTotal)
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}