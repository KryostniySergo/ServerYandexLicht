using Microsoft.EntityFrameworkCore;
using ServerViewYa.Models;

namespace ServerViewYa.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Audio> audios { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
