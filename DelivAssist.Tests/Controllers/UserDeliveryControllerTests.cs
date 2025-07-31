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
    public class UserDeliveryControllerTests 
    {
        private readonly UserDeliveryController _controller;
        private readonly ApplicationDbContext _context;

        public UserDeliveryControllerTests() {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            SeedDatabase();

            _controller = new UserDeliveryController(_context);

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

                _context.Deliveries.Add(delivery1);
                _context.SaveChanges();

                _context.UserDeliveries.Add(new UserDelivery 
                {
                    UserId = 1,
                    DeliveryId = 1
                });

                _context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetDeliveries_ReturnsUserDeliveries()
        {
            var results = await _controller.GetDeliveries();
            var okResult = Assert.IsType<OkObjectResult>(results);
            var userDeliveries = Assert.IsAssignableFrom<IEnumerable<DeliveryDto>>(okResult.Value);

            var userDelivery = Assert.Single(userDeliveries);
            Assert.Equal("Love Art Sushi", userDelivery.Restaurant);
        }

        [Fact]
        public async Task AddDelivery_AddsTo_ToUserList()
        {
            var delivery2 = new Delivery
            {
                Id = 2,
                App = DeliveryApp.Doordash,
                DeliveryTime = DateTime.Now,
                BasePay = 2.43,
                TipPay = 2.00,
                TotalPay = 4.43,
                Mileage = 1.4,
                Restaurant = "Serafina",
                CustomerNeighborhood = "Fenway",
                Notes = "test2"
            };

            var result = await _controller.AddDelivery(delivery2);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var userDelivery = await _context.UserDeliveries
                .FirstOrDefaultAsync(ud => ud.UserId == 1 && ud.DeliveryId == delivery2.Id);

            Assert.NotNull(userDelivery);
        }

        [Fact]
        public async Task GetFilteredDeliveries_ReturnsFilteredDeliveries()
        {
            var delivery3 = new Delivery
            {
                Id = 3,
                App = DeliveryApp.Grubhub,
                DeliveryTime = DateTime.Now,
                BasePay = 3.50,
                TipPay = 2.50,
                TotalPay = 6.00,
                Mileage = 0.9,
                Restaurant = "YGF Malatang",
                CustomerNeighborhood= "Mission Hill",
                Notes = "test 13"
            };

            _context.Deliveries.Add(delivery3);
            _context.UserDeliveries.Add(new UserDelivery
            {
                UserId = 1,
                DeliveryId = delivery3.Id
            });
            await _context.SaveChangesAsync();

            var result = await _controller.GetDeliveriesByApp(app: DeliveryApp.Grubhub,
                basePay: null,
                tipPay: null,
                totalPay: null,
                mileage: null,
                restaurant: null,
                customerNeighborhood: null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deliveries = Assert.IsAssignableFrom<IEnumerable<dynamic>>(okResult.Value);
            Assert.Contains(deliveries, d => (string)d.Restaurant == "YGF Malatang");
            Assert.DoesNotContain(deliveries, d => d.Id == 1);
            Assert.DoesNotContain(deliveries, d => (string)d.Restaurant == "Love Art Sushi");
        }
    }
}