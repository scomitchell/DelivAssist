namespace DelivAssist.Models
{
    public class UserShift
    {
        public int UserId { get; set; }

        public int ShiftId { get; set; }

        public DateTime DateAdded { get; set; }

        public User User { get; set; }

        public Shift Shift { get; set; }
    }
}