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
                ExpenseId = expense.Id,
                DateAdded = DateTime.UtcNow
            };

            _context.UserExpenses.Add(userExpense);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                expense.Amount,
                expense.Date,
                expense.Type,
                expense.Notes
            });
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
                    ue.Expense.Amount,
                    ue.Expense.Date,
                    ue.Expense.Type,
                    ue.Expense.Notes
                })
                .ToListAsync();

            return Ok(userExpenses);
        }

        [HttpGet("filtered-expenses")]
        public async Task<IActionResult> GetFilteredExpenses([FromQuery] double? amount,
            [FromQuery] DateTime? date,
            [FromQuery] string? type) 
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userExpensesQuery = _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .Include(ue => ue.Expense)
                .AsQueryable();

            if (amount.HasValue) {
                userExpensesQuery = userExpensesQuery.Where(ue => ue.Expense.Amount >= amount);
            }

            if (date.HasValue) {
                userExpensesQuery = userExpensesQuery.Where(ue => ue.Expense.Date >= date);
            }

            if (!string.IsNullOrEmpty(type)) {
                userExpensesQuery = userExpensesQuery.Where(ue => ue.Expense.Type == type);
            }

            var userExpenses = await userExpensesQuery
                .Select(ue => new {
                    ue.Expense.Id,
                    ue.Expense.Amount,
                    ue.Expense.Date,
                    ue.Expense.Type
                })
                .ToListAsync();

            return Ok(userExpenses);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetExpenseTypes() {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _context.UserExpenses
                .Where(ue => ue.UserId == userId)
                .Select(ue => ue.Expense.Type)
                .Distinct()
                .ToListAsync();
            
            return Ok(result);
        }

        [HttpGet("{expenseId:int}")]
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