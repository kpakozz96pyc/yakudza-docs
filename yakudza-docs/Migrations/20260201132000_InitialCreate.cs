using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace yakudza_docs.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DishTechCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishTechCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DishTechCardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    WeightGrams = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishIngredients_DishTechCards_DishTechCardId",
                        column: x => x.DishTechCardId,
                        principalTable: "DishTechCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Login = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DishTechCards",
                columns: new[] { "Id", "Description", "Image", "Name" },
                values: new object[,]
                {
                    { 1, "Классический украинский борщ с говядиной и сметаной", null, "Борщ" },
                    { 2, "Традиционный салат Оливье с курицей и майонезом", null, "Оливье" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "User" },
                    { 2, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "DishIngredients",
                columns: new[] { "Id", "DishTechCardId", "Name", "WeightGrams" },
                values: new object[,]
                {
                    { 1, 1, "Говядина", 300m },
                    { 2, 1, "Свекла", 200m },
                    { 3, 1, "Капуста", 150m },
                    { 4, 1, "Картофель", 200m },
                    { 5, 1, "Морковь", 100m },
                    { 6, 1, "Лук", 80m },
                    { 7, 1, "Томатная паста", 50m },
                    { 8, 1, "Сметана", 50m },
                    { 9, 2, "Куриное филе", 250m },
                    { 10, 2, "Картофель", 300m },
                    { 11, 2, "Морковь", 150m },
                    { 12, 2, "Яйца", 100m },
                    { 13, 2, "Огурцы маринованные", 100m },
                    { 14, 2, "Горошек консервированный", 80m },
                    { 15, 2, "Майонез", 120m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "PasswordHash", "PasswordSalt", "RoleId" },
                values: new object[,]
                {
                    { 1, "admin", new byte[] { 142, 79, 42, 59, 28, 93, 110, 127, 128, 145, 162, 179, 196, 213, 230, 247, 8, 25, 42, 59, 76, 93, 110, 127, 128, 145, 162, 179, 196, 213, 230, 247, 8, 25, 42, 59, 76, 93, 110, 127, 128, 145, 162, 179, 196, 213, 230, 247, 8, 25, 42, 59, 76, 93, 110, 127, 128, 145, 162, 179, 196, 213, 230, 247 }, new byte[] { 26, 43, 60, 77, 94, 111, 112, 129, 146, 163, 180, 197, 214, 231, 248, 9, 26, 43, 60, 77, 94, 111, 112, 129, 146, 163, 180, 197, 214, 231, 248, 9, 26, 43, 60, 77, 94, 111, 112, 129, 146, 163, 180, 197, 214, 231, 248, 9, 26, 43, 60, 77, 94, 111, 112, 129, 146, 163, 180, 197, 214, 231, 248, 9, 26, 43, 60, 77, 94, 111, 112, 129, 146, 163, 180, 197, 214, 231, 248, 9, 26, 43, 60, 77, 94, 111, 112, 129, 146, 163, 180, 197, 214, 231, 248, 9, 26, 43, 60, 77, 94, 111, 112, 129, 146, 163, 180, 197, 214, 231, 248, 9, 26, 43, 60, 77, 94, 111, 112, 129, 146, 163, 180, 197, 214, 231, 248, 9, 26, 43, 60, 77, 94, 111 }, 2 },
                    { 2, "user", new byte[] { 122, 107, 92, 77, 62, 47, 16, 1, 242, 227, 212, 197, 182, 167, 152, 137, 122, 107, 92, 77, 62, 47, 16, 1, 242, 227, 212, 197, 182, 167, 152, 137, 122, 107, 92, 77, 62, 47, 16, 1, 242, 227, 212, 197, 182, 167, 152, 137, 122, 107, 92, 77, 62, 47, 16, 1, 242, 227, 212, 197, 182, 167, 152, 137 }, new byte[] { 155, 140, 125, 110, 95, 64, 49, 34, 19, 4, 245, 230, 215, 200, 185, 170, 155, 140, 125, 110, 95, 64, 49, 34, 19, 4, 245, 230, 215, 200, 185, 170, 155, 140, 125, 110, 95, 64, 49, 34, 19, 4, 245, 230, 215, 200, 185, 170, 155, 140, 125, 110, 95, 64, 49, 34, 19, 4, 245, 230, 215, 200, 185, 170, 155, 140, 125, 110, 95, 64, 49, 34, 19, 4, 245, 230, 215, 200, 185, 170, 155, 140, 125, 110, 95, 64, 49, 34, 19, 4, 245, 230, 215, 200, 185, 170, 155, 140, 125, 110, 95, 64, 49, 34, 19, 4, 245, 230, 215, 200, 185, 170, 155, 140, 125, 110, 95, 64, 49, 34, 19, 4, 245, 230, 215, 200, 185, 170 }, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishIngredients_DishTechCardId",
                table: "DishIngredients",
                column: "DishTechCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishIngredients");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DishTechCards");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
