using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace T2Stats.Models
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<KillEvent> KillEvents { get; set; }
        public DbSet<EventReporter> EventReporters { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            // This space intentionally left blank.
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Set up keys
            builder.Entity<EventReporter>().HasKey(e => new { e.EventId, e.PlayerId }); // Composite key for many-to-many table
            builder.Entity<Player>().HasKey(p => p.PlayerId).ForSqlServerIsClustered(false); // Non-clustered on GUID
            builder.Entity<Event>().HasKey(e => e.EventId).ForSqlServerIsClustered(false); // Non-clustered on GUID
            builder.Entity<Match>().HasKey(m => m.MatchId).ForSqlServerIsClustered(false); // Non-clustered on GUID
            builder.Entity<Server>().HasKey(s => s.ServerId).ForSqlServerIsClustered(false); // Non-clustered on GUID

            // Set up clustered indexes
            builder.Entity<Player>().HasIndex(p => p.TribesGuid).ForSqlServerIsClustered(true);
            builder.Entity<Event>().HasIndex(e => e.MatchTime).ForSqlServerIsClustered(true);
            builder.Entity<Match>().HasIndex(m => m.StartTime).ForSqlServerIsClustered(true);
            builder.Entity<Server>().HasIndex(s => s.IpAddress).ForSqlServerIsClustered(true);
        }
    }
}