namespace DelivAssist.Models
{
    public class ShiftPredictionRequest
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string App { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
    }
}