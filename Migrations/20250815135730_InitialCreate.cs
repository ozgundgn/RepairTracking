using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairTracking.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    user_name = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    passive = table.Column<bool>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    surname = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: true),
                    code = table.Column<string>(type: "TEXT", unicode: false, maxLength: 4, nullable: true),
                    confirmed = table.Column<bool>(type: "INTEGER", nullable: true),
                    email = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pk_3", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    surname = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    passive = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_user = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk", x => x.id);
                    table.ForeignKey(
                        name: "customers_users_id_fk",
                        column: x => x.created_user,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    plate_number = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    chassis_no = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    customer_id = table.Column<int>(type: "INTEGER", nullable: false),
                    model = table.Column<int>(type: "INTEGER", nullable: true),
                    color = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: true),
                    type = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    engine_no = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    km = table.Column<int>(type: "INTEGER", nullable: true),
                    fuel = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: true),
                    passive = table.Column<bool>(type: "INTEGER", nullable: false),
                    image = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.id);
                    table.ForeignKey(
                        name: "vehicles_customers_id_fk",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "customers_vehicles",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    customer_id = table.Column<int>(type: "INTEGER", nullable: false),
                    vehicle_id = table.Column<int>(type: "INTEGER", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    passive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("customers_vehicles_pk", x => x.id);
                    table.ForeignKey(
                        name: "customers_vehicles_customers_id_fk",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "customers_vehicles_vehicles_id_fk",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "renovations",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    repair_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    delivery_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    vehicle_id = table.Column<int>(type: "INTEGER", nullable: false),
                    complaint = table.Column<string>(type: "TEXT", unicode: false, maxLength: 500, nullable: true),
                    note = table.Column<string>(type: "TEXT", unicode: false, maxLength: 500, nullable: true),
                    passive = table.Column<bool>(type: "INTEGER", nullable: true),
                    report_path = table.Column<string>(type: "TEXT", unicode: false, maxLength: 150, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("renovations_pk", x => x.id);
                    table.ForeignKey(
                        name: "renovations_vehicles_id_fk",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "renovation_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", unicode: false, maxLength: 500, nullable: true),
                    name = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: true),
                    price = table.Column<double>(type: "REAL", nullable: false),
                    tcode = table.Column<string>(name: "t-code", type: "TEXT", unicode: false, maxLength: 50, nullable: true),
                    note = table.Column<string>(type: "TEXT", unicode: false, maxLength: 500, nullable: true),
                    renovation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    passive = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("renovation_details_pk", x => x.id);
                    table.ForeignKey(
                        name: "renovation_details_renovations_id_fk",
                        column: x => x.renovation_id,
                        principalTable: "renovations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_created_user",
                table: "customers",
                column: "created_user");

            migrationBuilder.CreateIndex(
                name: "IX_customers_vehicles_customer_id",
                table: "customers_vehicles",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_vehicles_vehicle_id",
                table: "customers_vehicles",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_renovation_details_renovation_id",
                table: "renovation_details",
                column: "renovation_id");

            migrationBuilder.CreateIndex(
                name: "IX_renovations_vehicle_id",
                table: "renovations",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "users_pk",
                table: "users",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_pk_2",
                table: "users",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_customer_id",
                table: "vehicles",
                column: "customer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers_vehicles");

            migrationBuilder.DropTable(
                name: "renovation_details");

            migrationBuilder.DropTable(
                name: "renovations");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
