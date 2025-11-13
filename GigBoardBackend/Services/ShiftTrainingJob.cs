using GigBoardBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace GigBoardBackend.Services
{
    public class ShiftTrainingJob
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ShiftTrainingJob(ApplicationDbContext context, HttpClient client, IConfiguration config)
        {
            _context = context;
            _httpClient = client;
            _config = config;
        }

        public async Task TrainShiftModelJob()
        {
            Console.WriteLine("Starting shift model training job...");

            try
            {
                // Retrieve all shift/delivery data
                var shiftData = _context.ShiftDeliveries
                    .Where(sd => sd.Shift != null && sd.Delivery != null)
                    .Include(sd => sd.Shift)
                    .Include(sd => sd.Delivery)
                    .AsEnumerable()
                    .GroupBy(sd => new
                    {
                        sd.Shift!.Id,
                        sd.Shift.StartTime,
                        sd.Shift.EndTime,
                        sd.Shift.App,
                    })
                    .Select(g => new
                    {
                        g.Key.StartTime,
                        g.Key.EndTime,
                        g.Key.App,
                        Neighborhoods = g.Select(x => x.Delivery!.CustomerNeighborhood).Distinct().ToList(),
                        TotalEarnings = g.Sum(x => x.Delivery!.TotalPay),
                    })
                    .ToList();

                if (!shiftData.Any())
                {
                    Console.WriteLine("No shift data found to train on");
                    return;
                }

                var samples = shiftData.Select(d => new
                {
                    start_time = d.StartTime.ToString("HH:mm"),
                    end_time = d.EndTime.ToString("HH:mm"),
                    app = d.App.ToString(),
                    neighborhoods = d.Neighborhoods,
                    earnings = d.TotalEarnings
                });

                var payload = new { samples };

                // Python service URL
                var pythonServiceUrl = _config["PYTHON_SERVICE_URL"] ?? "http://localhost:8001";
                var response = await _httpClient.PostAsJsonAsync($"{pythonServiceUrl}/train/shift-model", payload);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Training API error: {response.StatusCode}");
                    return;
                }

                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Shift model training complete: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during shift model training: {ex.Message}");
            }
        }
    }
}