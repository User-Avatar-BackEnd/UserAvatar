using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace UserAvatar.Dal.Migrations
{
    public partial class AddedIndexToEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Histories_Events_EventName",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_EventName",
                table: "Histories");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "Events");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_EventName",
                table: "Histories",
                column: "EventName");

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_Events_EventName",
                table: "Histories",
                column: "EventName",
                principalTable: "Events",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
