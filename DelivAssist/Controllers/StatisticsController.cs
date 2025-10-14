using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using DelivAssist.Data;
using DelivAssist.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Builder.Extensions;

namespace DelivAssist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        private readonly string _pythonServiceUrl;

        public StatisticsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _pythonServiceUrl = Environment.GetEnvironmentVariable("PYTHON_SERVICE_URL") 
                ?? "http://localhost:8001";
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
        public async Task<IActionResult> GetAverageMonthlySpending()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .GroupBy(ue => new { ue.Expense.Date.Year, ue.Expense.Date.Month })
                .Select(g => g.Sum(ue => ue.Expense.Amount))
                .Average();

            if (result == null)
            {
                return NotFound("No expenses found");
            }

            return Ok(result);
        }

        [HttpGet("expenses/average-spending-by-type")]
        public async Task<IActionResult> GetAverageSpendingByType()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExpenses = await _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .Select(ue => new
                {
                    ue.Expense.Type,
                    ue.Expense.Amount,
                    Month = ue.Expense.Date.Month,
                    Year = ue.Expense.Date.Year
                })
                .ToListAsync();

            var totalMonths = userExpenses
                .Select(e => new { e.Year, e.Month })
                .Distinct()
                .Count();

            var result = userExpenses
                .GroupBy(e => e.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    AvgExpense = totalMonths > 0 ? g.Sum(x => x.Amount) / totalMonths : 0
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("shifts/average-shift-length")]
        public async Task<IActionResult> getAverageShiftLength()
        {
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
        public async Task<IActionResult> getAppWithMostShifts()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserShifts
                .Where(us => us.UserId == userId)
                .GroupBy(us => us.Shift.App)
                .Select(g => new
                {
                    App = g.Key,
                    ShiftCount = g.Count()
                })
                .OrderByDescending(g => g.ShiftCount)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return Ok(null);
            }

            return Ok(result.App);
        }

        [HttpGet("deliveries/restaurant-with-most-deliveries")]
        public async Task<IActionResult> GetRestaurantWithMostDeliveries()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .GroupBy(ud => ud.Delivery.Restaurant)
                .Select(g => new
                {
                    Restaurant = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(g => g.OrderCount)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound("No deliveries found");
            }

            return Ok(new
            {
                restaurant = result.Restaurant,
                orderCount = result.OrderCount
            });
        }

        [HttpGet("deliveries/tip-per-mile")]
        public async Task<IActionResult> GetTipPerMile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var miles = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .SumAsync(ud => ud.Delivery.Mileage);

            var tipPay = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .SumAsync(ud => ud.Delivery.TipPay);

            if (miles == 0 || tipPay == 0)
            {
                return Ok(0);
            }

            var result = tipPay / miles;

            return Ok(result);
        }

        [HttpGet("shifts/average-num-deliveries")]
        public async Task<IActionResult> GetAverageDeliveriesPerShift()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var shiftDeliveryCounts = await _context.ShiftDeliveries
                .Where(sd => sd.UserId == userId)
                .GroupBy(sd => sd.ShiftId)
                .Select(g => g.Count())
                .ToListAsync();

            if (!shiftDeliveryCounts.Any())
            {
                return Ok(0);
            }

            var average = shiftDeliveryCounts.Average();

            return Ok(average);
        }

        [HttpGet("charts/earnings-over-time")]
        public async Task<IActionResult> GetEarningsChart()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var deliveries = await (
                from ud in _context.UserDeliveries
                join d in _context.Deliveries on ud.DeliveryId equals d.Id
                where ud.UserId == userId
                group d by d.DeliveryTime.Date into g
                orderby g.Key
                select new
                {
                    Date = g.Key,
                    TotalEarnings = g.Sum(x => x.TotalPay)
                }
            ).ToListAsync();

            if (deliveries.Count == 0)
            {
                return NotFound("No deliveries found for user");
            }

            var dates = deliveries.Select(d => d.Date.ToString("yyyy-MM-dd")).ToList();
            var earnings = deliveries.Select(d => (double)d.TotalEarnings).ToList();

            var payload = new
            {
                dates = dates,
                earnings = earnings
            };

            var response = await _httpClient.PostAsJsonAsync($"{_pythonServiceUrl}/charts/earnings", payload);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "Error generating chart from Python API");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (result == null || !result.ContainsKey("image"))
            {
                return StatusCode(500, "Invalid response from Python API");
            }

            // Return base64 image string
            return Ok(new { base64Image = result["image"] });
        }

        [HttpGet("charts/tip-neighborhoods")]
        public async Task<IActionResult> GetTipNeighborhoodsChart()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var deliveries = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId)
                .Select(ud => new
                {
                    Neighborhood = ud.Delivery.CustomerNeighborhood.Trim(),
                    ud.Delivery.TipPay
                })
                .GroupBy(x => x.Neighborhood)
                .Select(g => new
                {
                    CustomerNeighborhood = g.Key,
                    AverageTipPay = g.Average(x => x.TipPay)
                })
                .OrderBy(x => x.CustomerNeighborhood)
                .ToListAsync();

            if (deliveries.Count == 0)
            {
                return NotFound("No deliveries found");
            }

            Console.WriteLine($"Request started at {DateTime.Now:HH:mm:ss.fff}");
            foreach (var d in deliveries)
            {
                Console.WriteLine($"{d.CustomerNeighborhood}: {d.AverageTipPay}");
            }

            Console.WriteLine("Tip Neighborhoods:");
            foreach (var d in deliveries)
            {
                Console.WriteLine($"{d.CustomerNeighborhood}: {d.AverageTipPay}");
            }

            var neighborhoods = deliveries.Select(d => d.CustomerNeighborhood).ToList();
            var tipPays = deliveries.Select(d => (double)d.AverageTipPay).ToList();

            var payload = new
            {
                neighborhoods = neighborhoods,
                tipPays = tipPays
            };

            var response = await _httpClient.PostAsJsonAsync($"{_pythonServiceUrl}/charts/tips-neighborhood", payload);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "Error retrieving tip by neighborhood chart");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (result == null || !result.ContainsKey("image"))
            {
                return StatusCode(500, "Invalid response from Python API");
            }

            // Return base64 image string
            return Ok(new { base64Image = result["image"] });
        }

        [HttpGet("charts/apps-by-base")]
        public async Task<IActionResult> GetAppsByBaseChart()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var deliveries = await (
                from ud in _context.UserDeliveries
                join d in _context.Deliveries on ud.DeliveryId equals d.Id
                where ud.UserId == userId
                group d by d.App into g
                orderby g.Key
                select new
                {
                    App = g.Key,
                    BasePay = g.Average(x => x.BasePay)
                }
            ).ToListAsync();

            if (deliveries.Count == 0)
            {
                return NotFound("No deliveries found");
            }

            var apps = deliveries.Select(d => d.App.ToString()).ToList();
            var basePays = deliveries.Select(d => (double)d.BasePay).ToList();

            var payload = new
            {
                apps = apps,
                basePays = basePays
            };

            var response = await _httpClient.PostAsJsonAsync($"{_pythonServiceUrl}/charts/apps-by-base", payload);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "Error getting base by apps histogram");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (result == null || !result.ContainsKey("image"))
            {
                return StatusCode(500, "Invalid response from Python API");
            }

            // Return base64 image string
            return Ok(new { base64Image = result["image"] });
        }

        [HttpGet("charts/hourly-earnings")]
        public async Task<IActionResult> GetHourlyEarningsChart()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

            var hourlyEarnings = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId && ud.Delivery.DeliveryTime >= oneWeekAgo)
                .Select(ud => new
                {
                    Hour = ud.Delivery.DeliveryTime.Hour,
                    Earnings = ud.Delivery.TotalPay
                })
                .GroupBy(x => x.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    AverageEarnings = g.Average(x => x.Earnings)
                })
                .OrderBy(x => x.Hour)
                .ToListAsync();

            var allHours = Enumerable.Range(0, 24).ToList();
            var earningsByHour = allHours
                .Select(h => new
                {
                    Hour = h,
                    AverageEarnings = hourlyEarnings.FirstOrDefault(x => x.Hour == h)?.AverageEarnings ?? 0
                })
                .ToList();

            var hoursStrings = earningsByHour.Select(x => x.Hour.ToString("D2")).ToList();
            var earnings = earningsByHour.Select(x => x.AverageEarnings).ToList();

            var payload = new
            {
                hours = hoursStrings,
                earnings = earnings
            };

            var response = await _httpClient.PostAsJsonAsync($"{_pythonServiceUrl}/charts/hourly-earnings", payload);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "Error retrieving hourly pay chart");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (result == null || !result.ContainsKey("image"))
            {
                return StatusCode(500, "Invalid response from Python API");
            }

            return Ok(new { base64Image = result["image"] });
        }

        [HttpGet("train/shift-model")]
        public async Task<IActionResult> TrainShiftModel()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var shiftData = _context.ShiftDeliveries
                .Where(sd => sd.UserId == userId)
                .Include(sd => sd.Shift)
                .Include(sd => sd.Delivery)
                .AsEnumerable()
                .GroupBy(sd => new
                {
                    sd.Shift.Id,
                    sd.Shift.StartTime,
                    sd.Shift.EndTime,
                    sd.Shift.App,
                })
                .Select(g => new
                {
                    StartTime = g.Key.StartTime,
                    EndTime = g.Key.EndTime,
                    App = g.Key.App,
                    Neighborhoods = g.Select(x => x.Delivery.CustomerNeighborhood).Distinct().ToList(),
                    TotalEarnings = g.Sum(x => x.Delivery.TotalPay),
                })
                .ToList();

            var samples = shiftData.Select(d => new
            {
                start_time = d.StartTime.ToString("HH:mm"),
                end_time = d.EndTime.ToString("HH:mm"),
                app = d.App.ToString(),
                neighborhoods = d.Neighborhoods,
                earnings = d.TotalEarnings
            });

            var payload = new { samples };

            var response = await _httpClient.PostAsJsonAsync($"{_pythonServiceUrl}/train/shift-model", payload);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "Python Training API error");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            return Ok(result);
        }

        [HttpPost("predict/shift-earnings")]
        public async Task<IActionResult> PredictShiftEarnings([FromBody] ShiftPredictionRequest request)
        {
            var payload = new
            {
                start_time = request.StartTime.ToString("HH:mm"),
                end_time = request.EndTime.ToString("HH:mm"),
                app = request.App,
                neighborhood = request.Neighborhood
            };

            var response = await _httpClient.PostAsJsonAsync($"{_pythonServiceUrl}/predict/shift-earnings", payload);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "Python prediction API error");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            return Ok(result);
        }
    }
}