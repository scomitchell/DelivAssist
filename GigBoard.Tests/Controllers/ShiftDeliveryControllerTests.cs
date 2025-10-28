using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GigBoardBackend.Controllers;
using GigBoardBackend.Models;
using GigBoardBackend.Data;

namespace GigBoard.Tests.Controllers
{
    public class ShiftDeliveryControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ShiftDeliveryController _controller;

        public ShiftDeliveryControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            SeedDatabase();

            _controller = new ShiftDeliveryController(_context);

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

            if (!_context.Shifts.Any())
            {
                _context.Shifts.Add(new Shift {
                    Id = 1,
                    StartTime = DateTime.Now.AddHours(-1),
                    EndTime = DateTime.Now,
                    App = DeliveryApp.UberEats
                });

                _context.SaveChanges();

                _context.UserShifts.Add(new UserShift 
                {
                    UserId = 1,
                    ShiftId = 1
                });
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

                _context.ShiftDeliveries.Add(new ShiftDelivery 
                {
                    UserId = 1,
                    ShiftId = 1,
                    DeliveryId = 1
                });

                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task GetShiftDeliveries_ReturnsDeliveries()
        {
            var result = await _controller.GetDeliveriesForShift(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var deliveries = Assert.IsAssignableFrom<IEnumerable<DeliveryDto>>(okResult.Value);

            Assert.Contains(deliveries, d => d.BasePay == 3.0);
        }

        [Fact]
        public async Task AddShiftDelivery_PostsShiftDelivery()
        {
            var shift2 = new Shift
            {
                Id = 2,
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now.AddHours(1),
                App = DeliveryApp.UberEats
            };

            _context.Shifts.Add(shift2);

            var delivery2 = new Delivery 
            {
                Id = 2,
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

            _context.Deliveries.Add(delivery2);

            _context.UserDeliveries.Add(new UserDelivery{
                UserId = 1,
                DeliveryId = 2
            });

            await _context.SaveChangesAsync();

            var result = await _controller.AddShiftDelivery(2, 2);
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Contains(_context.ShiftDeliveries, s => s.DeliveryId == 2);
        }
    }
}