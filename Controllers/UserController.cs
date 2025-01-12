using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ChildCareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return Ok(await _userRepository.GetAllUsersAsync());
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = $"User with ID {id} not found." });
            return Ok(user);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (user == null) return BadRequest(new { message = "Invalid user data." });

            await _userRepository.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
            {
                return BadRequest(new { message = "UserId in URL and payload do not match." });
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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User loginDetails)
        {
            if (string.IsNullOrEmpty(loginDetails.Email) || string.IsNullOrEmpty(loginDetails.Password))
            {
                return BadRequest(new { message = "Email and Password are required for login." });
            }

            var user = await _userRepository.LoginAsync(loginDetails.Email, loginDetails.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.RoleId
            });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            if (newUser == null) return BadRequest(new { message = "Invalid registration data." });

            var user = await _userRepository.RegisterAsync(newUser);
            if (user == null)
            {
                return Conflict(new { message = "Email already exists." });
            }

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }
    }
}
