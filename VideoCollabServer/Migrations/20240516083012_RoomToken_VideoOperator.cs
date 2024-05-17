using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoCollabServer.Migrations
{
    /// <inheritdoc />
    public partial class RoomToken_VideoOperator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Links_InviteLinkId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_AspNetUsers_UsersId",
                table: "RoomUsers");

            migrationBuilder.DropTable(
                name: "Streams");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_InviteLinkId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "InviteLinkId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "RoomUsers",
                newName: "JoinedUsersId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomUsers_UsersId",
                table: "RoomUsers",
                newName: "IX_RoomUsers_JoinedUsersId");

            migrationBuilder.AddColumn<string>(
                name: "TextRoomToken",
                table: "Rooms",
                type: "TEXT",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoOperatorId",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_VideoOperatorId",
                table: "Rooms",
                column: "VideoOperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_VideoOperatorId",
                table: "Rooms",
                column: "VideoOperatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_AspNetUsers_JoinedUsersId",
                table: "RoomUsers",
                column: "JoinedUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_VideoOperatorId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_AspNetUsers_JoinedUsersId",
                table: "RoomUsers");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_VideoOperatorId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "TextRoomToken",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "VideoOperatorId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "JoinedUsersId",
                table: "RoomUsers",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomUsers_JoinedUsersId",
                table: "RoomUsers",
                newName: "IX_RoomUsers_UsersId");

            migrationBuilder.AddColumn<int>(
                name: "InviteLinkId",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Streams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<string>(type: "TEXT", nullable: true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Streams_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Streams_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_InviteLinkId",
                table: "Rooms",
                column: "InviteLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_OwnerId",
                table: "Streams",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_RoomId",
                table: "Streams",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Links_InviteLinkId",
                table: "Rooms",
                column: "InviteLinkId",
                principalTable: "Links",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_AspNetUsers_UsersId",
                table: "RoomUsers",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
