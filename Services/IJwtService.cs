using AugustaAlumniAPI.Models;

namespace AugustaAlumniAPI.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
