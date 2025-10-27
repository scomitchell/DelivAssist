namespace DelivAssist.Models
{
    public class UserExpense
    {
        public int UserId { get; set; }
        public int ExpenseId { get; set; }

        public DateTime DateAdded { get; set; }

        public User? User { get; set; }

        public Expense? Expense { get; set; }
    }
}