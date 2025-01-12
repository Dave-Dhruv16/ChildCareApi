using ChildCareApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChildCareApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task<int> UpdateUserAsync(User user);
        Task DeleteUserAsync(int id); 
        Task<User> LoginAsync(string email, string password);
        Task<User> RegisterAsync(User user);
    }
}
