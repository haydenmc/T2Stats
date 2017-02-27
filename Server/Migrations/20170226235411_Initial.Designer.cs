using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using T2Stats.Models;

namespace Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170226235411_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("T2Stats.Models.Event", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<TimeSpan>("MatchDuration");

                    b.Property<string>("MatchGameType")
                        .HasMaxLength(128);

                    b.Property<string>("MatchMapName")
                        .HasMaxLength(128);

                    b.Property<DateTimeOffset>("MatchStartTime");

                    b.Property<TimeSpan>("MatchTime");

                    b.Property<string>("ReporterName")
                        .HasMaxLength(128);

                    b.Property<string>("ReporterTribesGuid")
                        .HasMaxLength(16);

                    b.Property<string>("ServerIpAddress")
                        .HasMaxLength(16);

                    b.Property<string>("ServerName")
                        .HasMaxLength(128);

                    b.Property<int>("ServerPort")
                        .HasMaxLength(6);

                    b.HasKey("EventId");

                    b.HasIndex("MatchMapName");

                    b.HasIndex("ReporterName");

                    b.HasIndex("ReporterTribesGuid");

                    b.HasIndex("ServerIpAddress");

                    b.HasIndex("ServerName");

                    b.HasIndex("ServerPort");

                    b.ToTable("Events");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Event");
                });

            modelBuilder.Entity("T2Stats.Models.KillEvent", b =>
                {
                    b.HasBaseType("T2Stats.Models.Event");

                    b.Property<string>("KillType")
                        .HasMaxLength(128);

                    b.Property<string>("KillerName")
                        .HasMaxLength(128);

                    b.Property<string>("KillerTribesGuid")
                        .HasMaxLength(16);

                    b.Property<string>("VictimName")
                        .HasMaxLength(128);

                    b.Property<string>("VictimTribesGuid")
                        .HasMaxLength(16);

                    b.Property<string>("Weapon")
                        .HasMaxLength(128);

                    b.HasIndex("KillType");

                    b.HasIndex("KillerName");

                    b.HasIndex("KillerTribesGuid");

                    b.HasIndex("MatchGameType");

                    b.HasIndex("VictimName");

                    b.HasIndex("VictimTribesGuid");

                    b.HasIndex("Weapon");

                    b.ToTable("KillEvent");

                    b.HasDiscriminator().HasValue("KillEvent");
                });
        }
    }
}
