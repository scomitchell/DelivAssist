using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DelivAssist.Controllers;
using DelivAssist.Models;
using DelivAssist.Data;

namespace DelivAssist.Tests.Controllers
{
    public class StatisticsControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly StatisticsController _controller;

        public StatisticsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            SeedDatabase();

            _controller = new StatisticsController(_context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() {User = user}
            };
        }

        private void SeedDatabase()
        {
            if (!_context.Users.Any())
            {
                _context.Users.Add(new User { 
                    Id = 1, 
                    FirstName="John",
                    LastName="Smith",
                    Email = "test@example.com",
                    Username="jsmith",
                    Password="password123"});

                _context.SaveChanges();
            }

            if (!_context.Deliveries.Any())
            {
                var delivery1 = new Delivery 
                {
                    Id = 1,
                    App = DeliveryApp.UberEats,
                    DeliveryTime = DateTime.Now,
                    BasePay = 3.0,
                    TipPay = 2.50,
                    TotalPay = 5.50,
                    Mileage = 1.2,
                    Restaurant = "Love Art Sushi",
                    CustomerNeighborhood= "Back Bay",
                    Notes = "test 1"
                };

                var delivery2 = new Delivery
                {
                    Id = 2,
                    App = DeliveryApp.Doordash,
                    DeliveryTime = DateTime.Now,
                    BasePay = 4.50,
                    TipPay = 2.00,
                    TotalPay = 6.50,
                    Mileage = 1.5,
                    Restaurant = "Wendy's",
                    CustomerNeighborhood = "Mission Hill",
                    Notes = "test 2"
                };

                _context.Deliveries.AddRange(delivery1, delivery2);
                _context.SaveChanges();

                _context.UserDeliveries.Add(new UserDelivery 
                {
                    UserId = 1,
                    DeliveryId = 1
                });

                _context.UserDeliveries.Add(new UserDelivery
                {
                    UserId = 1,
                    DeliveryId = 2
                });

                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task GetAveragePay_CalculatesPay()
        {
            var result = await _controller.GetAvgDeliveryPay();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var avgPay = Assert.IsAssignableFrom<double>(okResult.Value);

            Assert.Equal(6, avgPay);
        }

        [Fact]
        public async Task GetAverageBase_CalculatesBasePay()
        {
            var result = await _controller.GetAvgBasePay();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var avgBase = Assert.IsAssignableFrom<double>(okResult.Value);

            Assert.Equal(3.75, avgBase);
        }

        [Fact]
        public async Task GetAverageTip_CalculatesTipPay()
        {
            var result = await _controller.GetAvgTipPay();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var avgTip = Assert.IsAssignableFrom<double>(okResult.Value);

            Assert.Equal(2.25, avgTip);
        }

        [Fact]
        public async Task GetBestNeighborhood_ReturnsNeighborhood()
        {
            var result = await _controller.GetHighestPayingNeighborhood();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;
            var dict = value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(value));

            Assert.Equal("Back Bay", (string)dict["neighborhood"]);
            Assert.Equal(2.50, dict["averageTipPay"]);
        }

        [Fact]
        public async Task GetBestRestaurant_ReturnsRestaurant()
        {
            var result = await _controller.GetHighestPayingRestaurant();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;
            var dict = value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(value));

            Assert.Equal("Wendy's", (string)dict["restaurant"]);
            Assert.Equal(6.50, dict["avgTotalPay"]);
        }

        [Fact]
        public async Task GetBestBaseApp_ReturnsApp()
        {
            var result = await _controller.GetHighestPayingBaseApp();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;
            var dict = value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(value));

            Assert.Equal(DeliveryApp.Doordash, dict["app"]);
            Assert.Equal(4.50, dict["avgBase"]);
        }

        [Fact]
        public async Task GetBestTipApp_ReturnsApp()
        {
            var result = await _controller.GetHighestPayingTipApp();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;
            var dict = value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(value));

            Assert.Equal(DeliveryApp.UberEats, dict["app"]);
            Assert.Equal(2.50, dict["avgTip"]);
        }

        [Fact]
        public async Task GetDollarPerMile_ReturnsAverage()
        {
            var result = await _controller.GetDollarPerMile();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dollarPerMile = Assert.IsAssignableFrom<double>(okResult.Value);

            Assert.Equal(4.44, dollarPerMile, precision: 2);
        }

        [Fact]
        public async Task GetRestaurant_WithMostDeliveries_ReturnsDeliveries()
        {
            var delivery3 = new Delivery
            {
                Id = 3,
                App = DeliveryApp.Grubhub,
                DeliveryTime = DateTime.Now,
                BasePay = 3.2,
                TipPay = 2.70,
                TotalPay = 5.90,
                Mileage = 1.2,
                Restaurant = "Love Art Sushi",
                CustomerNeighborhood= "Fenway",
                Notes = "test 1"
            };
            _context.Deliveries.Add(delivery3);

            _context.UserDeliveries.Add(new UserDelivery
            {
                UserId = 1,
                DeliveryId = 3
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetRestaurantWithMostDeliveries();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;
            var dict = value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(value));

            Assert.Equal("Love Art Sushi", (string)dict["restaurant"]);
            Assert.Equal(2, dict["orderCount"]);
        }
    }
}