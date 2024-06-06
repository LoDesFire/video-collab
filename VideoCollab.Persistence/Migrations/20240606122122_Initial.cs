using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoCollab.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_UserId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MovieRoom");

            migrationBuilder.DropTable(
                name: "RoomUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AspNetUsers",
                newName: "ConnectedRoomId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_UserId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_ConnectedRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Rooms_ConnectedRoomId",
                table: "AspNetUsers",
                column: "ConnectedRoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Rooms_ConnectedRoomId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ConnectedRoomId",
                table: "AspNetUsers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_ConnectedRoomId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_UserId");

            migrationBuilder.CreateTable(
                name: "MovieRoom",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieRoom", x => new { x.PlaylistId, x.RoomsId });
                    table.ForeignKey(
                        name: "FK_MovieRoom_Movies_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieRoom_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomUsers",
                columns: table => new
                {
                    ConnectedRoomsId = table.Column<string>(type: "TEXT", nullable: false),
                    JoinedUsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomUsers", x => new { x.ConnectedRoomsId, x.JoinedUsersId });
                    table.ForeignKey(
                        name: "FK_RoomUsers_AspNetUsers_JoinedUsersId",
                        column: x => x.JoinedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomUsers_Rooms_ConnectedRoomsId",
                        column: x => x.ConnectedRoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieRoom_RoomsId",
                table: "MovieRoom",
                column: "RoomsId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomUsers_JoinedUsersId",
                table: "RoomUsers",
                column: "JoinedUsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_UserId",
                table: "AspNetUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
