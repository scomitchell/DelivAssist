namespace DelivAssist.Models
{
    public class UserDelivery
    {
        public int UserId { get; set; }
        public int DeliveryId { get; set; }

        public DateTime DateAdded { get; set; }

        public User User { get; set; } = new();

        public Delivery Delivery { get; set; } = new();

    }
}