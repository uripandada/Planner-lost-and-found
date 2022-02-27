using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
	public partial class MajorCleaningsRestructuring : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// moved cleaning history events from CleaningPlanItem to Cleaning            
			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_history_events_cleaning_plan_items_cleaning_plan_ite~",
				table: "cleaning_history_events");

			// moved cleaning inspections from CleaningPlanItem to Cleaning
			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_inspections_cleaning_plan_items_cleaning_plan_item_id",
				table: "cleaning_inspections");

			// moved inspected by from CleaningPlanItem to Cleaning          
			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_plan_items_asp_net_users_inspected_by_id",
				table: "cleaning_plan_items");

			// moved cleaning status from CleaningPlanItem to Cleaning          
			migrationBuilder.DropColumn(
				name: "cleaning_status",
				table: "cleaning_plan_items");

			// moved is inspected from CleaningPlanItem to Cleaning           
			migrationBuilder.DropColumn(
				name: "is_inspected",
				table: "cleaning_plan_items");

			// moved is inspection required from CleaningPlanItem to Cleaning           
			migrationBuilder.DropColumn(
				name: "is_inspection_required",
				table: "cleaning_plan_items");

			//// moved is sent from CleaningPlanItem to Cleaning           
			//migrationBuilder.RenameColumn(        
			//    name: "is_sent",          
			//    table: "cleaning_plan_items",      
			//    newName: "is_postponer");            
			migrationBuilder.DropColumn(name: "is_sent", table: "cleaning_plan_items");

			migrationBuilder.AddColumn<bool>(name: "is_postponer", table: "cleaning_plan_items", nullable: true);
			migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_postponer = false;");
			migrationBuilder.AlterColumn<bool>(name: "is_postponer", table: "cleaning_plan_items", nullable: false);


			//// moved is ready for inspection from CleaningPlanItem to Cleaning            
			//migrationBuilder.RenameColumn(
			//name: "is_ready_for_inspection",
			//table: "cleaning_plan_items",
			//newName: "is_postponee");
			migrationBuilder.DropColumn(name: "is_ready_for_inspection", table: "cleaning_plan_items");

			migrationBuilder.AddColumn<bool>(name: "is_postponee", table: "cleaning_plan_items", nullable: true);
			migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_postponee = false;");
			migrationBuilder.AlterColumn<bool>(name: "is_postponee", table: "cleaning_plan_items", nullable: false);


			//// moved is inspection success from CleaningPlanItem to Cleaning           
			//migrationBuilder.RenameColumn(
			//name: "is_inspection_success",
			//table: "cleaning_plan_items",
			//newName: "is_planned");
			migrationBuilder.DropColumn(name: "is_inspection_success", table: "cleaning_plan_items");

			migrationBuilder.AddColumn<bool>(name: "is_planned", table: "cleaning_plan_items", nullable: true);
			migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_planned = false;");
			migrationBuilder.AlterColumn<bool>(name: "is_planned", table: "cleaning_plan_items", nullable: false);


			//// moved inspected by id from CleaningPlanItem to Cleaning          
			//migrationBuilder.RenameColumn(            
			//name: "inspected_by_id",
			//table: "cleaning_plan_items",
			//newName: "room_bed_id");           
			migrationBuilder.DropColumn(name: "inspected_by_id", table: "cleaning_plan_items");

			migrationBuilder.AddColumn<Guid>(name: "room_bed_id", table: "cleaning_plan_items", nullable: true, type: "uuid");
			migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET room_bed_id = null;");


			// renamed CleaningPlanItem.CleaningPluginName to CleaningPlanItem.Description
			migrationBuilder.RenameColumn(name: "cleaning_plugin_name", table: "cleaning_plan_items", newName: "description");


			////migrationBuilder.RenameIndex(
			////name: "ix_cleaning_plan_items_inspected_by_id",
			////table: "cleaning_plan_items",
			////newName: "ix_cleaning_plan_items_room_bed_id");           
			//migrationBuilder.DropIndex(
			//	name: "ix_cleaning_plan_items_inspected_by_id",
			//	table: "cleaning_plan_items");

			migrationBuilder.CreateIndex(
				name: "ix_cleaning_plan_items_room_bed_id",
				table: "cleaning_plan_items",
				column: "room_bed_id");


			//migrationBuilder.RenameColumn(
			//name: "cleaning_plan_item_id",
			//table: "cleaning_inspections",
			//newName: "cleaning_id");           
			migrationBuilder.DropColumn(name: "cleaning_plan_item_id", table: "cleaning_inspections");

			migrationBuilder.AddColumn<Guid>(name: "cleaning_id", table: "cleaning_inspections", nullable: false, type: "uuid",
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
			//migrationBuilder.Sql($"UPDATE public.cleaning_inspections SET cleaning_id = {Guid.Empty};");            
			//migrationBuilder.AlterColumn<bool>(name: "cleaning_id", table: "cleaning_inspections", nullable: false);


			////migrationBuilder.RenameIndex(
			////name: "ix_cleaning_inspections_cleaning_plan_item_id",
			////table: "cleaning_inspections",
			////newName: "ix_cleaning_inspections_cleaning_id");           
			//migrationBuilder.DropIndex(
			//			   name: "ix_cleaning_inspections_cleaning_plan_item_id",
			//			   table: "cleaning_inspections");

			migrationBuilder.CreateIndex(
			   name: "ix_cleaning_inspections_cleaning_id",
			   table: "cleaning_inspections",
			   column: "cleaning_id");


			//migrationBuilder.RenameColumn(
			//name: "cleaning_plan_item_id",
			//table: "cleaning_history_events",
			//newName: "cleaning_id");
			migrationBuilder.DropColumn(name: "cleaning_plan_item_id", table: "cleaning_history_events");

			// DELETE EVERYTHING FROM THE TABLE IF THE COLUMN cleaning_id DOESN'T EXIST
			// SELECT * 
			// FROM
			//	  public.cleaning_history_events
			// WHERE 
			// 	  NOT EXISTS(
			// 		SELECT * 
			// 		FROM information_schema.columns 
			// 		WHERE table_name='cleaning_history_events' and column_name='cleaning_id')

			migrationBuilder.Sql(@"
				DELETE FROM public.cleaning_history_events
				WHERE NOT EXISTS(
			 		SELECT * 
			 		FROM information_schema.columns 
			 		WHERE table_name='cleaning_history_events' and column_name='cleaning_id');
			");

			migrationBuilder.AddColumn<Guid>(name: "cleaning_id", table: "cleaning_history_events", nullable: false, type: "uuid",
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
			//migrationBuilder.Sql($"UPDATE public.cleaning_history_events SET cleaning_id = {Guid.Empty};");            
			//migrationBuilder.AlterColumn<bool>(name: "cleaning_id", table: "cleaning_history_events", nullable: false);            
			

			////migrationBuilder.RenameIndex(            
			////    name: "ix_cleaning_history_events_cleaning_plan_item_id",            
			////    table: "cleaning_history_events",            
			////    newName: "ix_cleaning_history_events_cleaning_id");           
			//migrationBuilder.DropIndex(
			//	name: "ix_cleaning_history_events_cleaning_plan_item_id",
			//	table: "cleaning_history_events");

			migrationBuilder.CreateIndex(
				name: "ix_cleaning_history_events_cleaning_id",
				table: "cleaning_history_events",
				column: "cleaning_id");


			// StartsAt set to nullable            
			migrationBuilder.AlterColumn<DateTime>(
				name: "starts_at",
				table: "cleaning_plan_items",
				type: "timestamp without time zone",
				nullable: true,
				oldClrType: typeof(DateTime),
				oldType: "timestamp without time zone");


			// EndsAt set to nullable
			migrationBuilder.AlterColumn<DateTime>(
				name: "ends_at",
				table: "cleaning_plan_items",
				type: "timestamp without time zone",
				nullable: true,
				oldClrType: typeof(DateTime),
				oldType: "timestamp without time zone");

			// DurationSec set to nullable
			migrationBuilder.AlterColumn<int>(
				name: "duration_sec",
				table: "cleaning_plan_items",
				type: "integer",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "integer");

			// CleaningPlanGroupId set to nullable
			migrationBuilder.AlterColumn<Guid>(
				name: "cleaning_plan_group_id",
				table: "cleaning_plan_items",
				type: "uuid",
				nullable: true,
				oldClrType: typeof(Guid),
				oldType: "uuid");

			// Added CleaningId column
			migrationBuilder.AddColumn<Guid>(
				name: "cleaning_id",
				table: "cleaning_plan_items",
				type: "uuid",
				nullable: true);








			//// Added CleaningPlanId column
			//migrationBuilder.AddColumn<Guid>(
			//	name: "cleaning_plan_id",
			//	table: "cleaning_plan_items",
			//	type: "uuid",
			//	nullable: false,
			//	defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			migrationBuilder.AddColumn<bool>(name: "cleaning_plan_id", table: "cleaning_plan_items", nullable: true, type: "uuid");
			migrationBuilder.Sql($@"
				UPDATE public.cleaning_plan_items cpi
				SET cleaning_plan_id = (SELECT g.cleaning_plan_id FROM public.cleaning_plan_groups g WHERE g.id = cpi.cleaning_plan_group_id);
			");
			migrationBuilder.AlterColumn<bool>(name: "cleaning_plan_id", table: "cleaning_plan_items", nullable: false, type: "uuid");






			// Added reference to postponee cleaning plan item
			migrationBuilder.AddColumn<Guid>(
				name: "postponee_cleaning_plan_item_id",
				table: "cleaning_plan_items",
				type: "uuid",
				nullable: true);

			// Added reference to postponer cleaning plan item
			migrationBuilder.AddColumn<Guid>(
				name: "postponer_cleaning_plan_item_id",
				table: "cleaning_plan_items",
				type: "uuid",
				nullable: true);

			migrationBuilder.CreateTable(
				name: "cleanings",
				columns: table => new
				{
					id = table.Column<Guid>(type: "uuid", nullable: false),
					description = table.Column<string>(type: "text", nullable: true),
					credits = table.Column<int>(type: "integer", nullable: true),
					is_active = table.Column<bool>(type: "boolean", nullable: false),
					is_custom = table.Column<bool>(type: "boolean", nullable: false),
					is_postponed = table.Column<bool>(type: "boolean", nullable: false),
					is_change_sheets = table.Column<bool>(type: "boolean", nullable: false),
					is_inspection_required = table.Column<bool>(type: "boolean", nullable: false),
					is_ready_for_inspection = table.Column<bool>(type: "boolean", nullable: false),
					is_inspected = table.Column<bool>(type: "boolean", nullable: false),
					is_inspection_success = table.Column<bool>(type: "boolean", nullable: false),
					inspected_by_id = table.Column<Guid>(type: "uuid", nullable: true),
					status = table.Column<string>(type: "text", nullable: false, defaultValue: "DRAFT"),
					starts_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
					ends_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
					duration_sec = table.Column<int>(type: "integer", nullable: false),
					cleaner_id = table.Column<Guid>(type: "uuid", nullable: false),
					room_id = table.Column<Guid>(type: "uuid", nullable: false),
					room_bed_id = table.Column<Guid>(type: "uuid", nullable: true),
					cleaning_plugin_id = table.Column<Guid>(type: "uuid", nullable: true),
					cleaning_plan_id = table.Column<Guid>(type: "uuid", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("pk_cleanings", x => x.id);
					table.ForeignKey(
						name: "fk_cleanings_asp_net_users_cleaner_id",
						column: x => x.cleaner_id,
						principalTable: "asp_net_users",
						principalColumn: "id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "fk_cleanings_asp_net_users_inspected_by_id",
						column: x => x.inspected_by_id,
						principalTable: "asp_net_users",
						principalColumn: "id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "fk_cleanings_cleaning_plans_cleaning_plan_id",
						column: x => x.cleaning_plan_id,
						principalTable: "cleaning_plans",
						principalColumn: "id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "fk_cleanings_cleaning_plugins_cleaning_plugin_id",
						column: x => x.cleaning_plugin_id,
						principalTable: "cleaning_plugins",
						principalColumn: "id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "fk_cleanings_room_beds_room_bed_id",
						column: x => x.room_bed_id,
						principalTable: "room_beds",
						principalColumn: "id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "fk_cleanings_rooms_room_id",
						column: x => x.room_id,
						principalTable: "rooms",
						principalColumn: "id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "ix_cleaning_plan_items_cleaning_id",
				table: "cleaning_plan_items",
				column: "cleaning_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleaning_plan_items_cleaning_plan_id",
				table: "cleaning_plan_items",
				column: "cleaning_plan_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleaning_plan_items_postponee_cleaning_plan_item_id",
				table: "cleaning_plan_items",
				column: "postponee_cleaning_plan_item_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleaning_plan_items_postponer_cleaning_plan_item_id",
				table: "cleaning_plan_items",
				column: "postponer_cleaning_plan_item_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleanings_cleaner_id",
				table: "cleanings",
				column: "cleaner_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleanings_cleaning_plan_id",
				table: "cleanings",
				column: "cleaning_plan_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleanings_cleaning_plugin_id",
				table: "cleanings",
				column: "cleaning_plugin_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleanings_inspected_by_id",
				table: "cleanings",
				column: "inspected_by_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleanings_room_bed_id",
				table: "cleanings",
				column: "room_bed_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleanings_room_id",
				table: "cleanings",
				column: "room_id");

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_history_events_cleanings_cleaning_id",
				table: "cleaning_history_events",
				column: "cleaning_id",
				principalTable: "cleanings",
				principalColumn: "id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_inspections_cleanings_cleaning_id",
				table: "cleaning_inspections",
				column: "cleaning_id",
				principalTable: "cleanings",
				principalColumn: "id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_plan_items_cleaning_plan_items_postponee_cleaning_~",
				table: "cleaning_plan_items",
				column: "postponee_cleaning_plan_item_id",
				principalTable: "cleaning_plan_items",
				principalColumn: "id",
				onDelete: ReferentialAction.SetNull);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_plan_items_cleaning_plan_items_postponer_cleaning_~",
				table: "cleaning_plan_items",
				column: "postponer_cleaning_plan_item_id",
				principalTable: "cleaning_plan_items",
				principalColumn: "id",
				onDelete: ReferentialAction.SetNull);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_plan_items_cleaning_plans_cleaning_plan_id",
				table: "cleaning_plan_items",
				column: "cleaning_plan_id",
				principalTable: "cleaning_plans",
				principalColumn: "id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_plan_items_cleanings_cleaning_id",
				table: "cleaning_plan_items",
				column: "cleaning_id",
				principalTable: "cleanings",
				principalColumn: "id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_plan_items_rooms_room_bed_id",
				table: "cleaning_plan_items",
				column: "room_bed_id",
				principalTable: "rooms",
				principalColumn: "id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_history_events_cleanings_cleaning_id",
				table: "cleaning_history_events");

			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_inspections_cleanings_cleaning_id",
				table: "cleaning_inspections");

			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_plan_items_cleaning_plan_items_postponee_cleaning_~",
				table: "cleaning_plan_items");

			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_plan_items_cleaning_plan_items_postponer_cleaning_~",
				table: "cleaning_plan_items");

			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_plan_items_cleaning_plans_cleaning_plan_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_plan_items_cleanings_cleaning_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropForeignKey(
				name: "fk_cleaning_plan_items_rooms_room_bed_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropTable(
				name: "cleanings");

			migrationBuilder.DropIndex(
				name: "ix_cleaning_plan_items_cleaning_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropIndex(
				name: "ix_cleaning_plan_items_cleaning_plan_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropIndex(
				name: "ix_cleaning_plan_items_postponee_cleaning_plan_item_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropIndex(
				name: "ix_cleaning_plan_items_postponer_cleaning_plan_item_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropColumn(
				name: "cleaning_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropColumn(
				name: "cleaning_plan_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropColumn(
				name: "postponee_cleaning_plan_item_id",
				table: "cleaning_plan_items");

			migrationBuilder.DropColumn(
				name: "postponer_cleaning_plan_item_id",
				table: "cleaning_plan_items");

			//migrationBuilder.RenameColumn(
			//	name: "room_bed_id",
			//	table: "cleaning_plan_items",
			//	newName: "inspected_by_id");
			migrationBuilder.DropColumn(name: "room_bed_id", table: "cleaning_plan_items");

			migrationBuilder.AddColumn<Guid>(name: "inspected_by_id", table: "cleaning_plan_items", nullable: true, type: "uuid");
			migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET inspected_by_id = null;");

			//migrationBuilder.RenameColumn(
			//	name: "is_postponer",
			//	table: "cleaning_plan_items",
			//	newName: "is_sent");

			migrationBuilder.DropColumn(name: "is_postponer", table: "cleaning_plan_items");

			migrationBuilder.AddColumn<bool>(name: "is_sent", table: "cleaning_plan_items", nullable: true);
			migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_sent = false;");
			migrationBuilder.AlterColumn<bool>(name: "is_sent", table: "cleaning_plan_items", nullable: false);

			//migrationBuilder.RenameColumn(
			//	name: "is_postponee",
			//	table: "cleaning_plan_items",
			//	newName: "is_ready_for_inspection");
			migrationBuilder.DropColumn(name: "is_postponee", table: "cleaning_plan_items");

			migrationBuilder.AddColumn<bool>(name: "is_ready_for_inspection", table: "cleaning_plan_items", nullable: true);
			migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_ready_for_inspection = false;");
			migrationBuilder.AlterColumn<bool>(name: "is_ready_for_inspection", table: "cleaning_plan_items", nullable: false);

			//migrationBuilder.RenameColumn(
			//	name: "is_planned",
			//	table: "cleaning_plan_items",
			//	newName: "is_inspection_success");
			migrationBuilder.DropColumn(name: "is_planned", table: "cleaning_plan_items");

			migrationBuilder.AddColumn<bool>(name: "is_inspection_success", table: "cleaning_plan_items", nullable: true);
			migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_inspection_success = false;");
			migrationBuilder.AlterColumn<bool>(name: "is_inspection_success", table: "cleaning_plan_items", nullable: false);

			migrationBuilder.RenameColumn(
				name: "description",
				table: "cleaning_plan_items",
				newName: "cleaning_plugin_name");

			//migrationBuilder.RenameIndex(
			//	name: "ix_cleaning_plan_items_room_bed_id",
			//	table: "cleaning_plan_items",
			//	newName: "ix_cleaning_plan_items_inspected_by_id");
			migrationBuilder.CreateIndex(
				name: "ix_cleaning_plan_items_inspected_by_id",
				table: "cleaning_plan_items",
				column: "inspected_by_id");

			//migrationBuilder.RenameColumn(
			//	name: "cleaning_id",
			//	table: "cleaning_inspections",
			//	newName: "cleaning_plan_item_id");
			migrationBuilder.DropColumn(name: "cleaning_id", table: "cleaning_inspections");

			migrationBuilder.AddColumn<Guid>(name: "cleaning_plan_item_id", table: "cleaning_inspections", nullable: false, type: "uuid",
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			migrationBuilder.RenameIndex(
				name: "ix_cleaning_inspections_cleaning_id",
				table: "cleaning_inspections",
				newName: "ix_cleaning_inspections_cleaning_plan_item_id");

			//migrationBuilder.RenameColumn(
			//	name: "cleaning_id",
			//	table: "cleaning_history_events",
			//	newName: "cleaning_plan_item_id");
			migrationBuilder.DropColumn(name: "cleaning_id", table: "cleaning_history_events");

			migrationBuilder.AddColumn<Guid>(name: "cleaning_plan_item_id", table: "cleaning_history_events", nullable: false, type: "uuid",
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

			//migrationBuilder.RenameIndex(
			//	name: "ix_cleaning_history_events_cleaning_id",
			//	table: "cleaning_history_events",
			//	newName: "ix_cleaning_history_events_cleaning_plan_item_id");

			migrationBuilder.CreateIndex(
				name: "ix_cleaning_history_events_cleaning_plan_item_id",
				table: "cleaning_history_events",
				column: "cleaning_plan_item_id");

			migrationBuilder.AlterColumn<DateTime>(
				name: "starts_at",
				table: "cleaning_plan_items",
				type: "timestamp without time zone",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
				oldClrType: typeof(DateTime),
				oldType: "timestamp without time zone",
				oldNullable: true);

			migrationBuilder.AlterColumn<DateTime>(
				name: "ends_at",
				table: "cleaning_plan_items",
				type: "timestamp without time zone",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
				oldClrType: typeof(DateTime),
				oldType: "timestamp without time zone",
				oldNullable: true);

			migrationBuilder.AlterColumn<int>(
				name: "duration_sec",
				table: "cleaning_plan_items",
				type: "integer",
				nullable: false,
				defaultValue: 0,
				oldClrType: typeof(int),
				oldType: "integer",
				oldNullable: true);

			migrationBuilder.AlterColumn<Guid>(
				name: "cleaning_plan_group_id",
				table: "cleaning_plan_items",
				type: "uuid",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
				oldClrType: typeof(Guid),
				oldType: "uuid",
				oldNullable: true);

			migrationBuilder.AddColumn<string>(
				name: "cleaning_status",
				table: "cleaning_plan_items",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<bool>(
				name: "is_inspected",
				table: "cleaning_plan_items",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "is_inspection_required",
				table: "cleaning_plan_items",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_history_events_cleaning_plan_items_cleaning_plan_ite~",
				table: "cleaning_history_events",
				column: "cleaning_plan_item_id",
				principalTable: "cleaning_plan_items",
				principalColumn: "id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_inspections_cleaning_plan_items_cleaning_plan_item_id",
				table: "cleaning_inspections",
				column: "cleaning_plan_item_id",
				principalTable: "cleaning_plan_items",
				principalColumn: "id",
				onDelete: ReferentialAction.Restrict);

			migrationBuilder.AddForeignKey(
				name: "fk_cleaning_plan_items_asp_net_users_inspected_by_id",
				table: "cleaning_plan_items",
				column: "inspected_by_id",
				principalTable: "asp_net_users",
				principalColumn: "id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}
