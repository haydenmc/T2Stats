using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace T2Stats.Models
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Kill> Kills { get; set; }
        public DbSet<KillType> KillTypes { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Server> Servers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            // This space intentionally left blank.
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Set up indexes
            builder.Entity<Player>().HasIndex(p => p.TribesGuid);
            builder.Entity<Player>().HasIndex(p => p.Name);
            builder.Entity<Weapon>().HasIndex(w => w.Name);
            builder.Entity<Map>().HasIndex(m => m.Name);
            builder.Entity<Match>().HasIndex(m => m.GameType);
            builder.Entity<Server>().HasIndex(s => s.Name);
            builder.Entity<Server>().HasIndex(s => s.IpAddress);
            builder.Entity<Server>().HasIndex(s => s.Port);
            builder.Entity<KillType>().HasIndex(k => k.Type);

            // Set up some special foreign keys
            // builder.Entity<Kill>()
            //     .HasOne<Player>(k => k.Reporter)
            //     .WithMany()
            //     .HasForeignKey(k => k.ReporterId)
            //     .OnDelete(DeleteBehavior.SetNull);
            // builder.Entity<Kill>()
            //     .HasOne<Player>(k => k.Killer)
            //     .WithMany()
            //     .HasForeignKey(k => k.KillerId)
            //     .OnDelete(DeleteBehavior.SetNull);
            // builder.Entity<Kill>()
            //     .HasOne<Player>(k => k.Victim)
            //     .WithMany()
            //     .HasForeignKey(k => k.VictimId)
            //     .OnDelete(DeleteBehavior.SetNull);
        }
    }
}