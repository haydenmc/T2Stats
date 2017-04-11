using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using T2Stats.Models;

namespace T2Stats.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170411045108_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("T2Stats.Models.Event", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<Guid>("MatchId");

                    b.Property<TimeSpan>("MatchTime");

                    b.HasKey("EventId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("MatchId");

                    b.HasIndex("MatchTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Events");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Event");
                });

            modelBuilder.Entity("T2Stats.Models.EventReporter", b =>
                {
                    b.Property<Guid>("EventId");

                    b.Property<Guid>("PlayerId");

                    b.HasKey("EventId", "PlayerId");

                    b.HasIndex("PlayerId");

                    b.ToTable("EventReporters");
                });

            modelBuilder.Entity("T2Stats.Models.Match", b =>
                {
                    b.Property<Guid>("MatchId")
                        .ValueGeneratedOnAdd();

                    b.Property<TimeSpan>("Duration");

                    b.Property<string>("GameType");

                    b.Property<string>("MapName");

                    b.Property<Guid>("ServerId");

                    b.Property<DateTimeOffset>("StartTime");

                    b.HasKey("MatchId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ServerId");

                    b.HasIndex("StartTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("T2Stats.Models.Player", b =>
                {
                    b.Property<Guid>("PlayerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("TribesGuid");

                    b.HasKey("PlayerId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("TribesGuid")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Players");
                });

            modelBuilder.Entity("T2Stats.Models.Server", b =>
                {
                    b.Property<Guid>("ServerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IpAddress")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<int>("Port")
                        .HasMaxLength(6);

                    b.HasKey("ServerId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("IpAddress")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("T2Stats.Models.KillEvent", b =>
                {
                    b.HasBaseType("T2Stats.Models.Event");

                    b.Property<string>("KillType")
                        .HasMaxLength(128);

                    b.Property<Guid?>("KillerId");

                    b.Property<Guid?>("VictimId");

                    b.Property<string>("Weapon")
                        .HasMaxLength(128);

                    b.HasIndex("KillerId");

                    b.HasIndex("VictimId");

                    b.ToTable("KillEvent");

                    b.HasDiscriminator().HasValue("KillEvent");
                });

            modelBuilder.Entity("T2Stats.Models.Event", b =>
                {
                    b.HasOne("T2Stats.Models.Match", "Match")
                        .WithMany("Events")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("T2Stats.Models.EventReporter", b =>
                {
                    b.HasOne("T2Stats.Models.Event", "Event")
                        .WithMany("EventReports")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("T2Stats.Models.Player", "Player")
                        .WithMany("EventReports")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("T2Stats.Models.Match", b =>
                {
                    b.HasOne("T2Stats.Models.Server", "Server")
                        .WithMany("Matches")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("T2Stats.Models.KillEvent", b =>
                {
                    b.HasOne("T2Stats.Models.Player", "Killer")
                        .WithMany()
                        .HasForeignKey("KillerId");

                    b.HasOne("T2Stats.Models.Player", "Victim")
                        .WithMany()
                        .HasForeignKey("VictimId");
                });
        }
    }
}
