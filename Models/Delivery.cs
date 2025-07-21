namespace DelivAssist.Models
{
    public enum DeliveryApp
    {
        Doordash,
        UberEats,
        InstaCart,
        Grubhub
    }
    public class Delivery
    {
        public int Id { get; set; }

        public DeliveryApp App { get; set; }

        public DateTime DeliveryTime { get; set; }

        public double BasePay { get; set; }

        public double TipPay { get; set; }

        public double TotalPay { get; set; }

        public double Mileage { get; set; }

        public string Restaurant { get; set; } = string.Empty;

        public string CustomerNeighborhood { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public List<UserDelivery> UserDeliveries { get; set; } = new();

        public List<ShiftDelivery> ShiftDeliveries { get; set; } = new();
    }
}