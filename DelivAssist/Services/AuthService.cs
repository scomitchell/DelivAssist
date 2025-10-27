using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DelivAssist.Models;

namespace DelivAssist.Services
{
	public class AuthService
	{
		public AuthService()
		{

		}

		public string GenerateToken(User user)
		{
			var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
				?? throw new InvalidOperationException("JWT_SECRET_KEY environment variable is not set.");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username)
			};

			var token = new JwtSecurityToken(
				issuer: "DelivAssist",
				audience: "DelivAssist",
				claims: claims,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}