namespace GigBoardBackend.Models 
{
    public class ShiftDto
    {
        public int Id {get; set;}
        public DateTime StartTime {get; set;}
        public DateTime EndTime {get; set;}
        public DeliveryApp App {get; set;}
    }
}