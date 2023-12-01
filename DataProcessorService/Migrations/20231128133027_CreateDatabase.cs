using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataProcessorService.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModuleCategories",
                columns: table => new
                {
                    ModuleCategoryID = table.Column<string>(type: "TEXT", nullable: false),
                    ModuleState = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleCategories", x => x.ModuleCategoryID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleCategories");
        }
    }
}
