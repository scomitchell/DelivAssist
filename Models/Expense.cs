namespace DelivAssist.Models
{
    public class Expense
    {
        public int Id { get; set; }
        
        public double Amount { get; set; }

        public string Notes { get; set; } = string.Empty;

        public List<UserExpense> UserExpenses { get; set; } = new();
    }
}