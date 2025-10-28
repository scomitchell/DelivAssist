namespace GigBoardBackend.Models
{
    public class Expense
    {
        public int Id { get; set; }
        
        public double Amount { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public List<UserExpense> UserExpenses { get; set; } = new();
    }
}