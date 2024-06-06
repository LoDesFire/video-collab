using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoCollabServer.Migrations
{
    /// <inheritdoc />
    public partial class RoomId_ToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TextRoomToken",
                table: "Rooms",
                newName: "TextRoomSecret");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectedRoomsId",
                table: "RoomUsers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Rooms",
                type: "TEXT",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "RoomsId",
                table: "MovieRoom",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TextRoomSecret",
                table: "Rooms",
                newName: "TextRoomToken");

            migrationBuilder.AlterColumn<int>(
                name: "ConnectedRoomsId",
                table: "RoomUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 256)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "RoomsId",
                table: "MovieRoom",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
