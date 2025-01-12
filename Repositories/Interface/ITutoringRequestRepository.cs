using ChildCareApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChildCareApi.Repositories.Interfaces
{
    public interface ITutoringRequestRepository
    {
        Task<IEnumerable<TutoringRequest>> GetAllRequestsAsync();
        Task<TutoringRequest> GetRequestByIdAsync(int requestId);
        Task<IEnumerable<TutoringRequest>> GetRequestsByStudentIdAsync(int studentId);
        Task AddRequestAsync(TutoringRequest request);
        Task UpdateRequestAsync(TutoringRequest request);
        Task DeleteRequestAsync(int requestId);
        Task<bool> AcceptRequestAsync(int requestId, int tutorId);
        Task RejectRequestAsync(int requestId);
    }
}
