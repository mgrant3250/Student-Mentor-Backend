using AugustaAlumniAPI.Dtos;
using AugustaAlumniAPI.Models;

namespace AugustaAlumniAPI.Services
{
    public interface IUserService
    {
        Task<User?> Authenticate(string email, string password);
        Task<User> Register(RegisterDto dto);
        Task<User?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> ArchiveUserAsync(int id);
        Task<bool> UpdateUserRoleAsync(int id, string role);
        Task<IEnumerable<User>> GetMentorsAsync();
    }
}
