using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace T2Stats.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TribesGuid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    ServerId = table.Column<Guid>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 128, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Port = table.Column<int>(maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ServerId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    GameType = table.Column<string>(nullable: true),
                    MapName = table.Column<string>(nullable: true),
                    ServerId = table.Column<Guid>(nullable: false),
                    StartTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Matches_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    MatchId = table.Column<Guid>(nullable: false),
                    MatchTime = table.Column<TimeSpan>(nullable: false),
                    KillType = table.Column<string>(maxLength: 128, nullable: true),
                    KillerId = table.Column<Guid>(nullable: true),
                    VictimId = table.Column<Guid>(nullable: true),
                    Weapon = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Events_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Players_KillerId",
                        column: x => x.KillerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Players_VictimId",
                        column: x => x.VictimId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventReporters",
                columns: table => new
                {
                    EventId = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventReporters", x => new { x.EventId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_EventReporters_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventReporters_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_MatchId",
                table: "Events",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_MatchTime",
                table: "Events",
                column: "MatchTime")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_KillerId",
                table: "Events",
                column: "KillerId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_VictimId",
                table: "Events",
                column: "VictimId");

            migrationBuilder.CreateIndex(
                name: "IX_EventReporters_PlayerId",
                table: "EventReporters",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_ServerId",
                table: "Matches",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_StartTime",
                table: "Matches",
                column: "StartTime")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_TribesGuid",
                table: "Players",
                column: "TribesGuid")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Servers_IpAddress",
                table: "Servers",
                column: "IpAddress")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventReporters");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Servers");
        }
    }
}
