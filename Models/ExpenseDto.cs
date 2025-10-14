namespace DelivAssist.Models 
{
    public class ExpenseDto 
    {
        public int Id {get; set;}
        public double Amount {get; set;}
        public DateTime Date {get; set;}
        public string Type {get; set;} = string.Empty;
        public string Notes {get; set;} = string.Empty;
    }
}