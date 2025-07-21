namespace DelivAssist.Models
{
    public class ShiftDelivery
    {
        public int ShiftId { get; set; }

        public int DeliveryId { get; set; }

        public Shift Shift { get; set; } = new();

        public Delivery Delivery { get; set; } = new();
    }
}