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

            if (!deliveries.Any())
            {
                return new
                {
                    avgPay = 0,
                    avgBase = 0,
                    avgTip = 0,
                    dollarPerMile = 0,
                    highestPayingRestaurant = ""
                };
            }

            var avgPay = deliveries.Average(x => x!.TotalPay);
            var avgBase = deliveries.Average(x => x!.BasePay);
            var avgTip = deliveries.Average(x => x!.TipPay);

            var highestPayingRestaurant = deliveries
                .GroupBy(d => d!.Restaurant)
                .Select(g => new { Restaurant = g.Key, AvgTotalPay = g.Average(x => x!.TotalPay)})
                .OrderByDescending(x => x.AvgTotalPay)
                .FirstOrDefault();

            var miles = deliveries.Sum(x => x!.Mileage);
            var dollars = deliveries.Sum(x => x!.TotalPay);

            var dollarPerMile = dollars / miles;

            return new
            {
                avgPay,
                avgBase,
                avgTip,
                highestPayingRestaurant,
                dollarPerMile
            };
        }
    }
}