using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBookModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Books",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Isbn",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Isbn",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Books",
                newName: "Name");
        }
    }
}
