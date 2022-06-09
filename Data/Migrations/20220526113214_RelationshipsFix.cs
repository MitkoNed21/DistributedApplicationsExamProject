using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class RelationshipsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoards_Users_UserId",
                table: "MessageBoards");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_UserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_MessageBoards_UserId",
                table: "MessageBoards");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MessageBoards");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MessageBoards");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MessageBoards");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Users",
                newName: "UpdatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Users",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Messages",
                newName: "UpdatedById");

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "MessageBoards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "MessageBoards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedById",
                table: "Messages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoards_CreatedById",
                table: "MessageBoards",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoards_UpdatedById",
                table: "MessageBoards",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoards_Users_CreatedById",
                table: "MessageBoards",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoards_Users_UpdatedById",
                table: "MessageBoards",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_CreatedById",
                table: "Messages",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoards_Users_CreatedById",
                table: "MessageBoards");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoards_Users_UpdatedById",
                table: "MessageBoards");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_CreatedById",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatedById",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_MessageBoards_CreatedById",
                table: "MessageBoards");

            migrationBuilder.DropIndex(
                name: "IX_MessageBoards_UpdatedById",
                table: "MessageBoards");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "MessageBoards");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "MessageBoards");

            migrationBuilder.RenameColumn(
                name: "UpdatedById",
                table: "Users",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Users",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedById",
                table: "Messages",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "MessageBoards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "MessageBoards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MessageBoards",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoards_UserId",
                table: "MessageBoards",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoards_Users_UserId",
                table: "MessageBoards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
