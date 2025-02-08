using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ChildCareApi.Services;
using System.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using ChildCareApi.Models;

namespace ChildCareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly JwtService _jwtService;
        private readonly string _connectionString;

        public AuthController(IConfiguration config, JwtService jwtService)
        {
            _config = config;
            _jwtService = jwtService;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthModel.UserRegisterRequest model)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("spInsertUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@RoleId", model.RoleId);

                    conn.Open();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());
                    if (result == -1)
                        return BadRequest("Email already in use.");
                }

                return Ok(new AuthModel.UserRegisterResponse
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    RoleId = model.RoleId,
                    Message = "User registered successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthModel.UserLoginRequest model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("spValidateUserLogin", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", model.Email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string storedPassword = reader["Password"].ToString();
                    if (BCrypt.Net.BCrypt.Verify(model.Password, storedPassword))
                    {
                        var userId = Convert.ToInt32(reader["UserId"]);
                        var email = reader["Email"].ToString();
                        var roleId = Convert.ToInt32(reader["RoleId"]);
                        var firstName = reader["FirstName"].ToString();
                        var lastName = reader["LastName"].ToString();
                        var fullName = $"{firstName} {lastName}".Trim();

                        string token = _jwtService.GenerateToken(email, userId, roleId.ToString());

                        Response.Cookies.Append("AuthToken", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, 
                            SameSite = SameSiteMode.Strict, 
                            Expires = DateTime.UtcNow.AddMinutes(60) 
                        });

                        return Ok(new AuthModel.UserLoginResponse
                        {
                            Token = token,
                            UserId = userId,
                            Email = email,
                            RoleId = roleId,
                            FullName = fullName
                        });
                    }
                }
            }
            return Unauthorized("Invalid credentials.");
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult GetProtectedData()
        {
            return Ok("This is a protected API endpoint.");
        }
    }
}
