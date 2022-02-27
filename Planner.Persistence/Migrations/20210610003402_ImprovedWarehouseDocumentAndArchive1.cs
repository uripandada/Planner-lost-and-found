using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ImprovedWarehouseDocumentAndArchive1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_pending",
                table: "warehouse_documents");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "warehouse_documents",
                newName: "reserved_quantity_change");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "warehouse_document_archives",
                newName: "reserved_quantity_change");

            migrationBuilder.AddColumn<int>(
                name: "available_quantity_before_change",
                table: "warehouse_documents",
                type: "integer",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "available_quantity_change",
                table: "warehouse_documents",
                type: "integer",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "warehouse_documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reserved_quantity_before_change",
                table: "warehouse_documents",
                type: "integer",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "available_quantity_before_change",
                table: "warehouse_document_archives",
                type: "integer",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "available_quantity_change",
                table: "warehouse_document_archives",
                type: "integer",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "warehouse_document_archives",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reserved_quantity_before_change",
                table: "warehouse_document_archives",
                type: "integer",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "available_quantity_before_change",
                table: "warehouse_documents");

            migrationBuilder.DropColumn(
                name: "available_quantity_change",
                table: "warehouse_documents");

            migrationBuilder.DropColumn(
                name: "note",
                table: "warehouse_documents");

            migrationBuilder.DropColumn(
                name: "reserved_quantity_before_change",
                table: "warehouse_documents");

            migrationBuilder.DropColumn(
                name: "available_quantity_before_change",
                table: "warehouse_document_archives");

            migrationBuilder.DropColumn(
                name: "available_quantity_change",
                table: "warehouse_document_archives");

            migrationBuilder.DropColumn(
                name: "note",
                table: "warehouse_document_archives");

            migrationBuilder.DropColumn(
                name: "reserved_quantity_before_change",
                table: "warehouse_document_archives");

            migrationBuilder.RenameColumn(
                name: "reserved_quantity_change",
                table: "warehouse_documents",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "reserved_quantity_change",
                table: "warehouse_document_archives",
                newName: "quantity");

            migrationBuilder.AddColumn<bool>(
                name: "is_pending",
                table: "warehouse_documents",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
