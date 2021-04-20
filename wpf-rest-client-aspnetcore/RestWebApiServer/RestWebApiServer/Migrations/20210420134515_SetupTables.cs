using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RestWebApiServer.Migrations
{

public partial class SetupTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lock_version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "creators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lock_version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_creators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "authors_books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    creator_id = table.Column<int>(type: "int", nullable: false),
                    book_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    sort = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors_books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_authors_books_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_authors_books_creators_creator_id",
                        column: x => x.creator_id,
                        principalTable: "creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

//            migrationBuilder.CreateIndex(
//                name: "IX_authors_books_book_id",
//                table: "authors_books",
//                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_authors_books_creator_id_book_id",
                table: "authors_books",
                columns: new[] { "creator_id", "book_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authors_books");

            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "creators");
        }
    }
}
