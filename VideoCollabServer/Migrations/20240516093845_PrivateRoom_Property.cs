using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoCollabServer.Migrations
{
    /// <inheritdoc />
    public partial class PrivateRoom_Property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Private",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Private",
                table: "Rooms");
        }
    }
}
