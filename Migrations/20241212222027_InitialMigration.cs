using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASPDispetcher.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Удаление внешнего ключа между Report и Master
            migrationBuilder.DropForeignKey(
                name: "FK_REPORT_MAKE_MASTER",
                table: "Report");

            // Удаление первичного ключа из Master
            migrationBuilder.DropPrimaryKey(
                name: "PK_MASTER",
                table: "Master");

            // Удаление индекса make_FK из таблицы Report
            migrationBuilder.DropIndex(
                name: "make_FK",
                table: "Report");

            // Изменение типа UserId в Master
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Master",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Восстановление первичного ключа в Master
            migrationBuilder.AddPrimaryKey(
                name: "PK_MASTER",
                table: "Master",
                column: "UserId");

            // Изменение типа UserId в Report
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Report",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Восстановление индекса make_FK
            migrationBuilder.CreateIndex(
                name: "make_FK",
                table: "Report",
                column: "UserId");

            // Восстановление внешнего ключа между Master и AspNetUsers
            migrationBuilder.AddForeignKey(
                name: "FK_Master_AspNetUsers_UserId",
                table: "Master",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Восстановление внешнего ключа между Report и AspNetUsers
            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_UserId",
                table: "Report",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Report state journal");

            migrationBuilder.DropTable(
                name: "Work journal");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Report states");

            migrationBuilder.DropTable(
                name: "Work");

            migrationBuilder.DropTable(
                name: "Помещение");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Work type");

            migrationBuilder.DropTable(
                name: "Master");

            migrationBuilder.DropTable(
                name: "Object");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Подразделение");
        }
    }
}
