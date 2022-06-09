using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class Add_AdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedById", "CreatedOn", "FirstName", "IsAdmin", "LastName", "Password", "UpdatedById", "UpdatedOn", "UserName" },
                values: new object[] { 1, null, null, "ADMIN", true, "ADMIN", "�c��Ay�~�!�^�妞A)�U�4#=)", null, null, "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
