using Microsoft.EntityFrameworkCore;

namespace T2Stats.Models
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Kill> Kills { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            // This space intentionally left blank.
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}