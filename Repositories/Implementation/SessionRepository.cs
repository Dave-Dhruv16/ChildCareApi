using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ChildCareApi.Repositories.Implementation
{
    public class SessionRepository : ISessionRepository
    {
        private readonly string _connectionString;

        public SessionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Session>> GetAllSessionsAsync()
        {
            var sessions = new List<Session>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetAllSessions", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        sessions.Add(new Session
                        {
                            SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                            RequestId = reader.GetInt32(reader.GetOrdinal("RequestId")),
                            TutorId = reader.GetInt32(reader.GetOrdinal("TutorId")),
                            StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                            StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                            EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                            LocationId = reader.GetInt32(reader.GetOrdinal("LocationId")), 
                        });
                    }
                }
            }

            return sessions;
        }

        public async Task<Session> GetSessionByIdAsync(int sessionId)
        {
            Session session = null;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetSessionById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SessionId", sessionId);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        session = new Session
                        {
                            SessionId = reader.GetInt32("SessionId"),
                            RequestId = reader.GetInt32("RequestId"),
                            TutorId = reader.GetInt32("TutorId"),
                            StudentId = reader.GetInt32("StudentId"),
                            StartTime = reader.GetDateTime("StartTime"),
                            EndTime = reader.GetDateTime("EndTime"),
                            LocationId = reader.GetInt32("LocationId")
                        };
                    }
                }
            }

            return session;
        }

        public async Task AddSessionAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spInsertSession", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RequestId", session.RequestId);
                command.Parameters.AddWithValue("@TutorId", session.TutorId);
                command.Parameters.AddWithValue("@StudentId", session.StudentId);
                command.Parameters.AddWithValue("@StartTime", session.StartTime);
                command.Parameters.AddWithValue("@EndTime", session.EndTime);
                command.Parameters.AddWithValue("@LocationId", session.LocationId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> UpdateSessionAsync(Session session)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spUpdateSession", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SessionId", session.SessionId);
                command.Parameters.AddWithValue("@StartTime", session.StartTime);
                command.Parameters.AddWithValue("@EndTime", session.EndTime);
                command.Parameters.AddWithValue("@LocationId", session.LocationId);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteSessionAsync(int sessionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spDeleteSession", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SessionId", sessionId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
