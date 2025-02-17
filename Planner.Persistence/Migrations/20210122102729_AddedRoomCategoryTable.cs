﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedRoomCategoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "rooms",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "room_categorys",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    is_public = table.Column<bool>(nullable: false),
                    is_private = table.Column<bool>(nullable: false),
                    credits = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_categorys", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_categorys_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_room_categorys_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_rooms_category_id",
                table: "rooms",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_categorys_created_by_id",
                table: "room_categorys",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_categorys_modified_by_id",
                table: "room_categorys",
                column: "modified_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_rooms_room_categorys_category_id",
                table: "rooms",
                column: "category_id",
                principalTable: "room_categorys",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rooms_room_categorys_category_id",
                table: "rooms");

            migrationBuilder.DropTable(
                name: "room_categorys");

            migrationBuilder.DropIndex(
                name: "ix_rooms_category_id",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "rooms");
        }
    }
}
