using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace ChildCareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpGet("CheckConnection")]
        public IActionResult CheckDatabaseConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, new { message = "Connection string is not configured in the application settings." });
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        return Ok(new { message = "Database connection is established successfully." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to connect to the database.",
                    error = ex.Message
                });
            }

            return StatusCode(500, new { message = "Unknown error occurred while checking database connection." });
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();

            // Exclude password hashes from the response
            var userList = users.Select(user => new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.RoleId,
                user.IsActive
            });

            return Ok(userList);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = $"User with ID {id} not found." });

            // Exclude the password from the response
            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.RoleId,
                user.IsActive
            });
        }

        [HttpGet("GetByIdWithPassword/{id}")]
        public async Task<IActionResult> GetUserByIdWithPassword(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = $"User with ID {id} not found." });

            // Return user data along with the decrypted password
            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.RoleId,
                user.IsActive,
                Password = user.Password
            });
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (user == null) return BadRequest(new { message = "Invalid user data." });

            // Hash the password using BCrypt with explicit namespace
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _userRepository.AddUserAsync(user);

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.RoleId,
                user.IsActive
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
            {
                return BadRequest(new { message = "UserId in URL and payload do not match." });
            }

            // Hash the password if provided in the update request
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            var result = await _userRepository.UpdateUserAsync(user);
            if (result > 0)
            {
                return NoContent();
            }
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = $"User with ID {id} not found." });

            await _userRepository.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
