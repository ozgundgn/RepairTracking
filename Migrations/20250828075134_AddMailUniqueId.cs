using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairTracking.Migrations
{
    /// <inheritdoc />
    public partial class AddMailUniqueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "mails_pk1",
                table: "mails",
                column: "id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "mails_pk1",
                table: "mails");
        }
    }
}
