namespace DelivAssist.Models
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;

        public UserDto User { get; set; } = new();
    }
}