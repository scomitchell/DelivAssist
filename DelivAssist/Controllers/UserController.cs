using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DelivAssist.Services;
using DelivAssist.Models;
using DelivAssist.Data;

namespace DelivAssist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _service;

        public UserController(ApplicationDbContext context, AuthService service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {   
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("Username is already taken");
            }

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("Email is already taken");
            }

            // Hash password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Add user to db
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Issue token
            var token = _service.GenerateToken(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username
            };

            var response = new TokenResponse { 
                Token = token,
                User = userDto
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
            {
                return Unauthorized("Wrong username or password");
            }

            var token = _service.GenerateToken(user);

            var userDto = new UserDto
            {
                Id = existingUser.Id,
                Username = existingUser.Username
            };

            var response = new TokenResponse { 
                Token = token,
                User = userDto
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // If user is changing their username, check to see if new username is already taken
            if (existingUser.Username != user.Username)
            {
                bool usernameExists = await _context.Users.AnyAsync(u => u.Username == user.Username);
                if (usernameExists)
                {
                    return BadRequest("Username is taken");
                }
            }

            if (existingUser.Email != user.Email)
            {
                bool emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (emailExists)
                {
                    return BadRequest("Email is taken");
                }
            }

            // Update user details
            existingUser.Username = user.Username;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;

            // Update password
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            var responseUser = new
            {
                existingUser.Id,
                existingUser.FirstName,
                existingUser.LastName,
                existingUser.Email,
                existingUser.Username
            };

            return Ok(responseUser);
        }

        [Authorize]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}