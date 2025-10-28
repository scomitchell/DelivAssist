namespace GigBoardBackend.Models
{
    public class Shift
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DeliveryApp App { get; set; }

        public List<UserShift> UserShifts { get; set; } = new();

        public List<ShiftDelivery> ShiftDeliveries { get; set; } = new();
    }
}