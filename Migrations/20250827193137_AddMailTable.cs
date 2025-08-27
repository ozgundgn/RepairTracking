using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairTracking.Migrations
{
    /// <inheritdoc />
    public partial class AddMailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mails",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    type = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    subject = table.Column<string>(type: "TEXT", unicode: false, maxLength: 200, nullable: false),
                    template = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mails_pk", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mails");
        }
    }
}
