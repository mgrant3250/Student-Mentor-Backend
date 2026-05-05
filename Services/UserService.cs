using AugustaAlumniAPI.Dtos;
using AugustaAlumniAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AugustaAlumniAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AlumniContext _context;

        public UserService(AlumniContext context)
        {
            _context = context;
        }

        public async Task<User?> Authenticate(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
                ? user
                : null;
        }

        public async Task<User> Register(RegisterDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "user",
                CurrentJobTitle = dto.CurrentJobTitle,
                Organization = dto.Organization,
                GraduationTerm = dto.GraduationTerm,
                WantsToMentor = dto.WantsToMentor,
                GitHubLink = dto.GitHubLink,
                LinkedInLink = dto.LinkedInLink,
                ProfileImageUrl = string.IsNullOrWhiteSpace(dto.ProfileImageUrl)
                    ? "https://www.gravatar.com/avatar/?d=mp"
                    : dto.ProfileImageUrl
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Where(u => !u.Archived).ToListAsync();
        }

        public async Task<bool> ArchiveUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Archived = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(int id, string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Role = role;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetMentorsAsync()
        {
            return await _context.Users
                .Where(u => u.WantsToMentor && !u.Archived)
                .ToListAsync();
        }
    }
}
