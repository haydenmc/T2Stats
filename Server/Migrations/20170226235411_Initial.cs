using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    MatchDuration = table.Column<TimeSpan>(nullable: false),
                    MatchGameType = table.Column<string>(maxLength: 128, nullable: true),
                    MatchMapName = table.Column<string>(maxLength: 128, nullable: true),
                    MatchStartTime = table.Column<DateTimeOffset>(nullable: false),
                    MatchTime = table.Column<TimeSpan>(nullable: false),
                    ReporterName = table.Column<string>(maxLength: 128, nullable: true),
                    ReporterTribesGuid = table.Column<string>(maxLength: 16, nullable: true),
                    ServerIpAddress = table.Column<string>(maxLength: 16, nullable: true),
                    ServerName = table.Column<string>(maxLength: 128, nullable: true),
                    ServerPort = table.Column<int>(maxLength: 6, nullable: false),
                    KillType = table.Column<string>(maxLength: 128, nullable: true),
                    KillerName = table.Column<string>(maxLength: 128, nullable: true),
                    KillerTribesGuid = table.Column<string>(maxLength: 16, nullable: true),
                    VictimName = table.Column<string>(maxLength: 128, nullable: true),
                    VictimTribesGuid = table.Column<string>(maxLength: 16, nullable: true),
                    Weapon = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_MatchMapName",
                table: "Events",
                column: "MatchMapName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ReporterName",
                table: "Events",
                column: "ReporterName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ReporterTribesGuid",
                table: "Events",
                column: "ReporterTribesGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ServerIpAddress",
                table: "Events",
                column: "ServerIpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ServerName",
                table: "Events",
                column: "ServerName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ServerPort",
                table: "Events",
                column: "ServerPort");

            migrationBuilder.CreateIndex(
                name: "IX_Events_KillType",
                table: "Events",
                column: "KillType");

            migrationBuilder.CreateIndex(
                name: "IX_Events_KillerName",
                table: "Events",
                column: "KillerName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_KillerTribesGuid",
                table: "Events",
                column: "KillerTribesGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Events_MatchGameType",
                table: "Events",
                column: "MatchGameType");

            migrationBuilder.CreateIndex(
                name: "IX_Events_VictimName",
                table: "Events",
                column: "VictimName");

            migrationBuilder.CreateIndex(
                name: "IX_Events_VictimTribesGuid",
                table: "Events",
                column: "VictimTribesGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Weapon",
                table: "Events",
                column: "Weapon");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
