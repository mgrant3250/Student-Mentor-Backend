using Microsoft.EntityFrameworkCore;

namespace AugustaAlumniAPI.Models
{
    public class AlumniContext : DbContext
    {

        public AlumniContext(DbContextOptions<AlumniContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
