using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using T2Stats.Models;

namespace Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170131073427_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("T2Stats.Models.Kill", b =>
                {
                    b.Property<Guid>("KillId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("KillerId");

                    b.Property<Guid?>("MatchId");

                    b.Property<TimeSpan>("MatchTime");

                    b.Property<Guid?>("ReporterId");

                    b.Property<Guid?>("VictimId");

                    b.Property<Guid?>("WeaponId");

                    b.HasKey("KillId");

                    b.HasIndex("KillerId");

                    b.HasIndex("MatchId");

                    b.HasIndex("ReporterId");

                    b.HasIndex("VictimId");

                    b.HasIndex("WeaponId");

                    b.ToTable("Kills");
                });

            modelBuilder.Entity("T2Stats.Models.Map", b =>
                {
                    b.Property<Guid>("MapId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.HasKey("MapId");

                    b.HasIndex("Name");

                    b.ToTable("Maps");
                });

            modelBuilder.Entity("T2Stats.Models.Match", b =>
                {
                    b.Property<Guid>("MatchId")
                        .ValueGeneratedOnAdd();

                    b.Property<TimeSpan>("Duration");

                    b.Property<string>("GameType")
                        .HasMaxLength(128);

                    b.Property<Guid?>("MapId");

                    b.Property<Guid?>("ServerId");

                    b.Property<DateTimeOffset>("StartTime");

                    b.HasKey("MatchId");

                    b.HasIndex("GameType");

                    b.HasIndex("MapId");

                    b.HasIndex("ServerId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("T2Stats.Models.Player", b =>
                {
                    b.Property<Guid>("PlayerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("TribesGuid")
                        .HasMaxLength(32);

                    b.HasKey("PlayerId");

                    b.HasIndex("Name");

                    b.HasIndex("TribesGuid");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("T2Stats.Models.Server", b =>
                {
                    b.Property<Guid>("ServerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IpAddress")
                        .HasMaxLength(16);

                    b.Property<int>("Port");

                    b.HasKey("ServerId");

                    b.HasIndex("IpAddress");

                    b.HasIndex("Port");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("T2Stats.Models.Weapon", b =>
                {
                    b.Property<Guid>("WeaponId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.HasKey("WeaponId");

                    b.HasIndex("Name");

                    b.ToTable("Weapons");
                });

            modelBuilder.Entity("T2Stats.Models.Kill", b =>
                {
                    b.HasOne("T2Stats.Models.Player", "Killer")
                        .WithMany()
                        .HasForeignKey("KillerId");

                    b.HasOne("T2Stats.Models.Match", "Match")
                        .WithMany("Kills")
                        .HasForeignKey("MatchId");

                    b.HasOne("T2Stats.Models.Player", "Reporter")
                        .WithMany()
                        .HasForeignKey("ReporterId");

                    b.HasOne("T2Stats.Models.Player", "Victim")
                        .WithMany()
                        .HasForeignKey("VictimId");

                    b.HasOne("T2Stats.Models.Weapon", "Weapon")
                        .WithMany()
                        .HasForeignKey("WeaponId");
                });

            modelBuilder.Entity("T2Stats.Models.Match", b =>
                {
                    b.HasOne("T2Stats.Models.Map", "Map")
                        .WithMany()
                        .HasForeignKey("MapId");

                    b.HasOne("T2Stats.Models.Server", "Server")
                        .WithMany("Matches")
                        .HasForeignKey("ServerId");
                });
        }
    }
}
