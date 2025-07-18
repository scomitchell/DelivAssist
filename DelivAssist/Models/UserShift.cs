namespace DelivAssist.Models
{
    public class UserShift
    {
        public int UserId { get; set; }

        public int ShiftId { get; set; }

        public DateTime DateAdded { get; set; }

        public User User { get; set; } = new();

        public Shift Shift { get; set; } = new();
    }
}