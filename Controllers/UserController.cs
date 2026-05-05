using AugustaAlumniAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AugustaAlumniAPI.Dtos;
using BCrypt.Net;
using AugustaAlumniAPI.Services;

namespace AugustaAlumniAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            //if (string.IsNullOrWhiteSpace(dto.Password))
            //    return BadRequest("Password is required.");

            //if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            //    return BadRequest("Email already registered.");

            //var user = new User
            //{
            //    FirstName = dto.FirstName,
            //    LastName = dto.LastName,
            //    StudentId = dto.StudentId,
            //    Email = dto.Email,
            //    PhoneNumber = dto.PhoneNumber,
            //    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            //    //PasswordHash = dto.Password,
            //    CurrentJobTitle = dto.CurrentJobTitle,
            //    Organization = dto.Organization,
            //    GraduationTerm = dto.GraduationTerm,
            //    WantsToMentor = dto.WantsToMentor,
            //    GitHubLink = dto.GitHubLink,
            //    LinkedInLink = dto.LinkedInLink,
            //    //ProfileImageUrl = dto.ProfileImageUrl
            //    ProfileImageUrl = string.IsNullOrWhiteSpace(dto.ProfileImageUrl)
            //    ? "https://www.gravatar.com/avatar/?d=mp"
            //    : dto.ProfileImageUrl
            //};

            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            //return Ok(new { message = "User registered successfully." });

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required.");

            try
            {
                var user = await _userService.Register(dto);
                return Ok(new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            //if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            //    return Unauthorized("Invalid credentials.");

            //var token = GenerateJwtToken(user);
            //return Ok(new { token, role = user.Role });

            var user = await _userService.Authenticate(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token, role = user.Role });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            //if (userIdClaim == null)
            //    return Unauthorized("Invalid token.");

            //int userId = int.Parse(userIdClaim.Value);
            //var user = await _context.Users.FindAsync(userId);

            //if (user == null)
            //    return NotFound("User not found.");


            //return Ok(new
            //{
            //    user.Id,
            //    user.FirstName,
            //    user.LastName,
            //    user.Email,
            //    user.CurrentJobTitle,
            //    user.Organization,
            //    user.GraduationTerm,
            //    user.WantsToMentor,
            //    user.GitHubLink,
            //    user.LinkedInLink,
            //    user.ProfileImageUrl,
            //    user.Role
            //});

            var user = await _userService.GetByIdAsync(GetUserIdFromToken());
            if (user == null) return Unauthorized("Invalid token or user not found.");

            return Ok(UserToDto(user));
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteOwnAccount()
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            //if (userIdClaim == null) return Unauthorized("Invalid token.");

            //int userId = int.Parse(userIdClaim.Value);

            //var user = await _context.Users.FindAsync(userId);
            //if (user == null) return NotFound("User not found.");

            //_context.Users.Remove(user);
            //await _context.SaveChangesAsync();

            //return Ok(new { message = "Your account has been deleted." });

            var userId = GetUserIdFromToken();
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return Unauthorized("Invalid token or user not found.");

            await _userService.DeleteAsync(userId);
            return Ok(new { message = "Your account has been deleted." });
        }

        
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            //var users = await _context.Users
            //    .Where(u => !u.Archived)
            //    .ToListAsync();

            //return Ok(users);

            var users = await _userService.GetAllUsersAsync();
            return Ok(users.Select(UserToDto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //var user = await _context.Users.FindAsync(id);
            //if (user == null) return NotFound();

            //_context.Users.Remove(user);
            //await _context.SaveChangesAsync();

            //return Ok(new { message = "User deleted successfully." });

            var success = await _userService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "User deleted successfully." });


        }

        
        [Authorize(Roles = "admin")]
        [HttpPut("archive/{id}")]
        public async Task<IActionResult> ArchiveUser(int id)
        {
            //var user = await _context.Users.FindAsync(id);
            //if (user == null) return NotFound();

            //user.Archived = true;
            //await _context.SaveChangesAsync();

            //return Ok(new { message = "User archived." });

            var success = await _userService.ArchiveUserAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "User archived." });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("role/{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string role)
        {
            //var user = await _context.Users.FindAsync(id);
            //if (user == null) return NotFound();

            //user.Role = role;
            //await _context.SaveChangesAsync();
            //return Ok(new { message = $"User role updated to {role}." });

            var success = await _userService.UpdateUserRoleAsync(id, role);
            if (!success) return NotFound();
            return Ok(new { message = $"User role updated to {role}." });
        }

        
        [HttpGet("mentors")]
        public async Task<IActionResult> GetMentors()
        {
            //var mentors = await _context.Users
            //    .Where(u => u.WantsToMentor && !u.Archived)
            //    .ToListAsync();

            //return Ok(mentors);

            var mentors = await _userService.GetMentorsAsync();
            return Ok(mentors.Select(UserToDto));
        }


        //private string GenerateJwtToken(User user)
        //{
        //    var jwtKey = _config["Jwt:Key"];
        //    if (string.IsNullOrEmpty(jwtKey))
        //        throw new Exception("JWT key is not configured");
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //    new Claim(ClaimTypes.Email, user.Email),
        //    new Claim(ClaimTypes.Role, user.Role)
        //};

        //    var token = new JwtSecurityToken(
        //        _config["Jwt:Issuer"],
        //        _config["Jwt:Issuer"],
        //        claims,
        //        expires: DateTime.Now.AddDays(7),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private object UserToDto(User user) => new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.CurrentJobTitle,
            user.Organization,
            user.GraduationTerm,
            user.WantsToMentor,
            user.GitHubLink,
            user.LinkedInLink,
            user.ProfileImageUrl,
            user.Role
        };
    }
}