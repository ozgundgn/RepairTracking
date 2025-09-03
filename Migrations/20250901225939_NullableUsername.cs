using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairTracking.Migrations
{
    /// <inheritdoc />
    public partial class NullableUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "users",
                type: "TEXT",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "users",
                type: "TEXT",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
