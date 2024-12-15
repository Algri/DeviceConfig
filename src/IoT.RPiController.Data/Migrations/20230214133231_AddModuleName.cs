using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoT.RPiController.Data.Migrations
{
    public partial class AddModuleName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.CreateTable(
                name: "ModuleConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<byte>(type: "INTEGER", nullable: false),
                    PortA = table.Column<byte>(type: "INTEGER", nullable: false),
                    PortB = table.Column<byte>(type: "INTEGER", nullable: false),
                    ModuleName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleConfigurations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleConfigurations");

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<byte>(type: "INTEGER", nullable: false),
                    PortA = table.Column<byte>(type: "INTEGER", nullable: false),
                    PortB = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });
        }
    }
}
