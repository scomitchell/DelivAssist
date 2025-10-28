namespace GigBoardBackend.Models 
{
    public class DeliveryDto
    {
        public int Id {get; set;}
        public DeliveryApp App {get; set;}
        public DateTime DeliveryTime {get; set;}
        public double TotalPay {get; set;}
        public double BasePay {get; set;}
        public double TipPay {get; set;}
        public double Mileage {get; set;}
        public string Restaurant {get; set;} = string.Empty;
        public string CustomerNeighborhood {get; set;} = string.Empty;
        public string Notes {get; set;} = string.Empty;
    }
}