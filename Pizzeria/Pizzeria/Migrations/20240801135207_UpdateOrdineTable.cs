using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrdineTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleUser",
                table: "RoleUser");

            migrationBuilder.DropIndex(
                name: "IX_RoleUser_UsersId",
                table: "RoleUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleUser",
                table: "RoleUser",
                columns: new[] { "UsersId", "RolesId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_RolesId",
                table: "RoleUser",
                column: "RolesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleUser",
                table: "RoleUser");

            migrationBuilder.DropIndex(
                name: "IX_RoleUser_RolesId",
                table: "RoleUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleUser",
                table: "RoleUser",
                columns: new[] { "RolesId", "UsersId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                table: "RoleUser",
                column: "UsersId");
        }
    }
}
