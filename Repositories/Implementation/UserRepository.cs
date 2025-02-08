using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ChildCareApi.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetAllUsers", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            UserId = reader.GetInt32("UserId"),
                            Email = reader.GetString("Email"),
                            Password = reader.GetString("Password"), 
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            RoleId = reader.GetInt32("RoleId"),
                            IsActive = reader.GetBoolean("IsActive"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });

                    }
                }
            }

            return users;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            User user = null;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetUserById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            UserId = reader.GetInt32("UserId"),
                            Email = reader.GetString("Email"),
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            RoleId = reader.GetInt32("RoleId"),
                            IsActive = reader.GetBoolean("IsActive"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };
                    }
                }
            }

            return user;
        }

        public async Task AddUserAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("spInsertUser", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@RoleId", user.RoleId);

            connection.Open();
            await command.ExecuteNonQueryAsync();
        }
        public async Task<int> UpdateUserAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spUpdateUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", user.UserId);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@RoleId", user.RoleId);
                command.Parameters.AddWithValue("@IsActive", user.IsActive);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("spDeleteUser", connection) 
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", id);

            connection.Open();
            await command.ExecuteNonQueryAsync();
        }
        public async Task<User> LoginAsync(string email, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("spGetUserByEmail", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Email", email);

            connection.Open();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var storedPassword = reader["Password"].ToString();
                if (storedPassword == password) 
                {
                    return new User
                    {
                        UserId = (int)reader["UserId"],
                        Email = reader["Email"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        RoleId = (int)reader["RoleId"],
                        IsActive = (bool)reader["IsActive"]
                    };
                }
            }

            return null; 
        }
        public async Task<User> RegisterAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("spInsertUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@RoleId", user.RoleId);

            var returnParameter = new SqlParameter
            {
                ParameterName = "@ReturnValue",
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParameter);

            connection.Open();
            await command.ExecuteNonQueryAsync();

            var result = (int)returnParameter.Value;

            if (result == -1)
            {
                return null; 
            }

            user.UserId = result;
            return user;
        }

    }
}
