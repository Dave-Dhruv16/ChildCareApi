using ChildCareApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChildCareApi.Repositories.Interfaces
{
    public interface ITutorRepository
    {
        Task<IEnumerable<Tutor>> GetAllTutorsAsync();
        Task<Tutor?> GetTutorByIdAsync(int tutorId);
        Task<Tutor?> GetTutorByUserIdAsync(int tutorId);
        Task<int> AddTutorAsync(Tutor tutor);
        Task<int> UpdateTutorAsync(Tutor tutor);
        Task<int> DeleteTutorAsync(int tutorId);
    }
}
