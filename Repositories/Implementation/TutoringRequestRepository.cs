using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ChildCareApi.Repositories.Implementation
{
    public class TutoringRequestRepository : ITutoringRequestRepository
    {
        private readonly string _connectionString;

        public TutoringRequestRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<TutoringRequest>> GetAllRequestsAsync()
        {
            var requests = new List<TutoringRequest>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetAllTutoringRequests", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        requests.Add(new TutoringRequest
                        {
                            RequestId = reader.GetInt32("RequestId"),
                            StudentId = reader.GetInt32("StudentId"),
                            Subject = reader.GetString("Subject"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            RequestedAt = reader.GetDateTime("RequestedAt"),
                            Status = reader.GetString("Status"),
                            TutorId = reader.IsDBNull("TutorId") ? null : reader.GetInt32("TutorId"),
                            AcceptedAt = reader.IsDBNull("AcceptedAt") ? null : reader.GetDateTime("AcceptedAt")
                        });
                    }
                }
            }

            return requests;
        }

        public async Task<TutoringRequest> GetRequestByIdAsync(int requestId)
        {
            TutoringRequest request = null;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetTutoringRequestById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RequestId", requestId);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        request = new TutoringRequest
                        {
                            RequestId = reader.GetInt32("RequestId"),
                            StudentId = reader.GetInt32("StudentId"),
                            Subject = reader.GetString("Subject"),
                            Description = reader.GetString("Description"),
                            RequestedAt = reader.GetDateTime("RequestedAt"),
                            Status = reader.GetString("Status"),
                            TutorId = reader.IsDBNull("TutorId") ? null : reader.GetInt32("TutorId"),
                            AcceptedAt = reader.IsDBNull("AcceptedAt") ? null : reader.GetDateTime("AcceptedAt")
                        };
                    }
                }
            }

            return request;
        }

        public async Task<IEnumerable<TutoringRequest>> GetRequestsByStudentIdAsync(int studentId)
        {
            var requests = new List<TutoringRequest>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetTutoringRequestsByStudentId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StudentId", studentId); // Ensuring that studentId is passed as a parameter

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())  // Reading the data row by row
                    {
                        requests.Add(new TutoringRequest
                        {
                            RequestId = reader.GetInt32("RequestId"),
                            StudentId = reader.GetInt32("StudentId"),
                            Subject = reader.GetString("Subject"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            RequestedAt = reader.GetDateTime("RequestedAt"),
                            Status = reader.GetString("Status"),
                            TutorId = reader.IsDBNull("TutorId") ? null : reader.GetInt32("TutorId"),  // Handle NULL for TutorId
                            AcceptedAt = reader.IsDBNull("AcceptedAt") ? null : reader.GetDateTime("AcceptedAt")
                        });
                    }
                }
            }

            return requests;
        }

        public async Task AddRequestAsync(TutoringRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Subject))
            {
                throw new ArgumentException("Subject is required.");
            }

            if (request.StudentId <= 0)
            {
                throw new ArgumentException("Invalid Student ID.");
            }

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("spInsertTutoringRequest", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StudentId", request.StudentId);
            command.Parameters.AddWithValue("@Subject", request.Subject);
            command.Parameters.AddWithValue("@Description", (object?)request.Description ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateRequestAsync(TutoringRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spUpdateTutoringRequest", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RequestId", request.RequestId);
                command.Parameters.AddWithValue("@Subject", request.Subject);
                command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteRequestAsync(int requestId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spDeleteTutoringRequest", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RequestId", requestId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<TutoringRequest>> GetRequestsByTutorIdAsync(int tutorId)
        {
            var requests = new List<TutoringRequest>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetRequestsByTutorId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TutorId", tutorId);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        requests.Add(new TutoringRequest
                        {
                            RequestId = reader.GetInt32("RequestId"),
                            StudentId = reader.GetInt32("StudentId"),
                            Subject = reader.GetString("Subject"),
                            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                            RequestedAt = reader.GetDateTime("RequestedAt"),
                            Status = reader.GetString("Status"),
                            TutorId = reader.IsDBNull("TutorId") ? null : reader.GetInt32("TutorId"),
                            AcceptedAt = reader.IsDBNull("AcceptedAt") ? null : reader.GetDateTime("AcceptedAt")
                        });
                    }
                }
            }

            return requests;
        }


        public async Task<bool> AcceptRequestAsync(int requestId, int tutorId)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("spAcceptRequest", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@RequestId", requestId);
            command.Parameters.AddWithValue("@TutorId", tutorId);

            connection.Open();

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task RejectRequestAsync(int requestId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spRejectRequest", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RequestId", requestId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
