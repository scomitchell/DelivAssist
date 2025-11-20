using System.Globalization;
using GigBoardBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace GigBoardBackend.Services
{
    public class StatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> CalculateStatistics(int userId)
        {
            var deliveries = await _context.UserDeliveries
                .Where(ud => ud.UserId == userId && ud.Delivery != null)
                .Select(ud => ud.Delivery)
                .ToListAsync();

            // If no deliveries default to 0, empty list, or ""
            if (!deliveries.Any())
            {
                return new
                {
                    avgPay = 0,
                    avgBase = 0,
                    avgTip = 0,
                    dollarPerMile = 0,
                    tipPerMile = 0,
                    highestPayingRestaurant = new {restaurant = "", avgTotalPay = 0},
                    plotlyEarningsData = new {dates = new List<string>(), earnings = new List<double>()}
                };
            }
            
            // Average total, base, and tip
            var avgPay = deliveries.Average(x => x!.TotalPay);
            var avgBase = deliveries.Average(x => x!.BasePay);
            var avgTip = deliveries.Average(x => x!.TipPay);

            // Highest paying restaurant with total
            var highestPayingRestaurant = deliveries
                .GroupBy(d => d!.Restaurant)
                .Select(g => new { Restaurant = g.Key, AvgTotalPay = g.Average(x => x!.TotalPay)})
                .OrderByDescending(x => x.AvgTotalPay)
                .FirstOrDefault();

            // Dollar-per-mile and tip-per-mile
            var miles = deliveries.Sum(x => x!.Mileage);
            var dollarsTotal = deliveries.Sum(x => x!.TotalPay);
            var dollarsTip = deliveries.Sum(x => x!.TipPay);

            var dollarPerMile = dollarsTotal / miles;
            var tipPerMile = dollarsTip / miles;

            // Plotly Earnings Data
            var plotlyEarnings = deliveries
                .GroupBy(d => d!.DeliveryTime.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalEarnings = g.Sum(x => x!.TotalPay)
                })
                .ToList();

            var dates = plotlyEarnings.Select(d => d.Date.ToString("yyyy-MM-dd")).ToList();
            var earnings = plotlyEarnings.Select(d => (double)d.TotalEarnings).ToList();

            var plotlyEarningsData = new {dates, earnings};

            // Return all stats
            return new
            {
                avgPay,
                avgBase,
                avgTip,
                highestPayingRestaurant,
                dollarPerMile,
                tipPerMile,
                plotlyEarningsData
            };
        }
    }
}