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
    public class UserExpenseControllerTests : IDisposable
    {
        private readonly UserExpenseController _controller;
        private readonly ApplicationDbContext _context;

        public UserExpenseControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            _context = new ApplicationDbContext(options);

            SeedDatabase();

            _controller = new UserExpenseController(_context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext {User = user}
            };
        }

        private void SeedDatabase()
        {
            if (!_context.Users.Any())
            {
                _context.Users.Add(new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "example@email.com",
                    Username = "jsmith",
                    Password = "password123"
                });

                _context.SaveChanges();
            }

            if (!_context.Expenses.Any())
            {
                var expense1 = new Expense
                {
                    Id = 1,
                    Amount = 30,
                    Date = DateTime.Now,
                    Type = "Gas",
                    Notes = "Test1"
                };

                _context.Expenses.Add(expense1);
                _context.SaveChanges();

                _context.UserExpenses.Add(new UserExpense
                {
                    UserId = 1,
                    ExpenseId = 1
                });

                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task GetExpenses_ReturnsExpenses()
        {
            var result = await _controller.GetExpenses();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var userExpenses = Assert.IsAssignableFrom<IEnumerable<ExpenseDto>>(okResult.Value);
            
            var userExpense = Assert.Single(userExpenses);
            Assert.Equal(30, userExpense.Amount);
        }

        [Fact]
        public async Task DeleteExpense_RemovesExpense()
        {
            Assert.True(_context.Expenses.Any(e => e.Id == 1));
            var result = await _controller.DeleteExpense(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.False(_context.Expenses.Any());
        }
    }
}