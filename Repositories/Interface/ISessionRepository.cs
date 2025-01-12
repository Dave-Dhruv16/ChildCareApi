using ChildCareApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChildCareApi.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        Task<IEnumerable<Session>> GetAllSessionsAsync();
        Task<Session> GetSessionByIdAsync(int sessionId);
        Task AddSessionAsync(Session session);
        Task<int> UpdateSessionAsync(Session session);
        Task DeleteSessionAsync(int sessionId);
    }
}
