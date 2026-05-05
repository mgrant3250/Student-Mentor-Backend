namespace AugustaAlumniAPI.Dtos
{
    public class RegisterDto
    {
        public string StudentId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public string Password { get; set; } = string.Empty;

        public string? CurrentJobTitle { get; set; }
        public string? Organization { get; set; }

        public string? GraduationTerm { get; set; }

        public bool WantsToMentor { get; set; }

        public string? GitHubLink { get; set; }
        public string? LinkedInLink { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
