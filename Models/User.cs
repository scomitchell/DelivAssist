namespace DelivAssist.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public List<UserDelivery> UserDeliveries { get; set; } = new();

        public List<UserExpense> UserExpenses { get; set; } = new();

        public List<UserShift> UserShifts { get; set; } = new();

    }
}