using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using DelivAssist.Data;
using DelivAssist.Models;

namespace DelivAssist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserExpenseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] Expense expense)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            var userExpense = new UserExpense
            {
                UserId = userId,
                ExpenseId = expense.Id
            };

            _context.UserExpenses.Add(userExpense);
            await _context.SaveChangesAsync();

            return Ok("Expense Added");
        }

        [HttpGet("my-expenses")]
        public async Task<IActionResult> GetExpenses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExpenses = await _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .Include(ue => ue.Expense)
                .Select(ue => new
                {
                    ue.Expense.Id,
                    ue.Expense.Amount
                })
                .ToListAsync();

            return Ok(userExpenses);
        }

        [HttpGet("{expenseId}")]
        public async Task<IActionResult> GetExpenseById(int expenseId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExpense = await _context.UserExpenses
                .FirstOrDefaultAsync(us => us.UserId == userId && us.ExpenseId == expenseId);

            if (userExpense == null)
            {
                return NotFound("Expense not found");
            }

            return Ok(userExpense);
        }

        [HttpDelete("{expenseId}")]
        public async Task<IActionResult> DeleteExpense(int expenseId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExpense = await _context.UserExpenses
                .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.ExpenseId == expenseId);

            if (userExpense == null)
            {
                return NotFound("Expense Not found");
            }

            _context.UserExpenses.Remove(userExpense);
            await _context.SaveChangesAsync();

            return Ok("Expense Deleted");
        }
    }

}