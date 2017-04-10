using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace T2Stats.Models
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Player> Players { get; set; }
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
            builder.Entity<EventReporter>().HasKey(e => new { e.EventId, e.PlayerId });
            builder.Entity<Player>().HasKey(p => p.PlayerId).ForSqlServerIsClustered(false); // Non-clustered on GUID
            builder.Entity<Event>().HasKey(e => e.EventId).ForSqlServerIsClustered(false); // Non-clustered on GUID

            // Set up indexes
            // Clustered indexes
            builder.Entity<Player>().HasIndex(p => p.TribesGuid).ForSqlServerIsClustered(true);
            builder.Entity<Event>().HasIndex(e => e.MatchTime).ForSqlServerIsClustered(true);
            // Event indexes
            builder.Entity<Event>().HasIndex(e => e.MatchMapName);
            builder.Entity<Event>().HasIndex(e => e.ServerIpAddress);
            builder.Entity<Event>().HasIndex(e => e.ServerName);
            builder.Entity<Event>().HasIndex(e => e.ServerPort);
            // Kill event indexes
            builder.Entity<KillEvent>().HasIndex(e => e.KillerName);
            builder.Entity<KillEvent>().HasIndex(e => e.KillerTribesGuid);
            builder.Entity<KillEvent>().HasIndex(e => e.KillType);
            builder.Entity<KillEvent>().HasIndex(e => e.MatchGameType);
            builder.Entity<KillEvent>().HasIndex(e => e.MatchMapName);
            builder.Entity<KillEvent>().HasIndex(e => e.VictimName);
            builder.Entity<KillEvent>().HasIndex(e => e.VictimTribesGuid);
            builder.Entity<KillEvent>().HasIndex(e => e.Weapon);
        }
    }
}