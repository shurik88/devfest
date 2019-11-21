using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DisciplinesService.Migrations
{
    public partial class addDiscipline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Annotion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_Code",
                table: "Disciplines",
                column: "Code",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disciplines");
        }
    }
}
