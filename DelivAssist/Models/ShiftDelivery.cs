namespace DelivAssist.Models
{
    public class ShiftDelivery
    {
        public int ShiftId { get; set; }

        public int UserId {get; set;}

        public int DeliveryId { get; set; }

        public Shift? Shift { get; set; }

        public User? User {get; set;}

        public Delivery? Delivery { get; set; }
    }
}