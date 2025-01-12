using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ChildCareApi.Repositories.Implementation
{
    public class TutorRepository : ITutorRepository
    {
        private readonly string _connectionString;

        public TutorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Tutor>> GetAllTutorsAsync()
        {
            var tutors = new List<Tutor>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetAllTutors", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tutors.Add(new Tutor
                        {
                            TutorId = reader.GetInt32("TutorId"),
                            UserId = reader.GetInt32("UserId"),
                            Qualifications = reader["Qualifications"] as string,
                            Specialization = reader["Specialization"] as string,
                            ExperienceYears = reader["ExperienceYears"] as int?
                        });
                    }
                }
            }

            return tutors;
        }

        public async Task<Tutor?> GetTutorByIdAsync(int tutorId)
        {
            Tutor? tutor = null;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spGetTutorById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TutorId", tutorId);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        tutor = new Tutor
                        {
                            TutorId = reader.GetInt32("TutorId"),
                            UserId = reader.GetInt32("UserId"),
                            Qualifications = reader["Qualifications"] as string,
                            Specialization = reader["Specialization"] as string,
                            ExperienceYears = reader["ExperienceYears"] as int?
                        };
                    }
                }
            }

            return tutor;
        }

        public async Task<int> AddTutorAsync(Tutor tutor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("spInsertTutor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", tutor.UserId);
            command.Parameters.AddWithValue("@Qualifications", (object?)tutor.Qualifications ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specialization", (object?)tutor.Specialization ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceYears", (object?)tutor.ExperienceYears ?? DBNull.Value);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateTutorAsync(Tutor tutor)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("spUpdateTutor", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TutorId", tutor.TutorId);
                command.Parameters.AddWithValue("@UserId", tutor.UserId);
                command.Parameters.AddWithValue("@Qualifications", tutor.Qualifications);
                command.Parameters.AddWithValue("@Specialization", tutor.Specialization);
                command.Parameters.AddWithValue("@ExperienceYears", tutor.ExperienceYears);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> DeleteTutorAsync(int tutorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("spDeleteTutor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@TutorId", tutorId);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }
    }
}
