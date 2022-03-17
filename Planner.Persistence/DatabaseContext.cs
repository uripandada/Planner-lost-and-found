using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using Planner.Common.Shared;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Persistence
{

	public class DatabaseContext : IdentityDbContext<User, Role, Guid>, IDatabaseContext, IPersistedGrantDbContext
	{
		public DbSet<NumberOfTasksPerUser> NumberOfTasksPerUser { get; set; }

		public DbSet<PersistedGrant> PersistedGrants { get; set; }
		public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
		public DbSet<UserGroup> UserGroups { get; set; }
		public DbSet<UserSubGroup> UserSubGroups { get; set; }
		public DbSet<Hotel> Hotels { get; set; }
		public DbSet<Settings> Settings { get; set; }
		public DbSet<Area> Areas { get; set; }
		public DbSet<Building> Buildings { get; set; }
		public DbSet<Floor> Floors { get; set; }
		public DbSet<Room> Rooms { get; set; }
		//public DbSet<RoomWithHotelStructureView> RoomsWithHotelStructureView { get; set; }
		public DbSet<RoomCategory> RoomCategorys { get; set; }
		public DbSet<LostAndFoundCategory> LostAndFoundCategories { get; set; }
		public DbSet<Experience> Experiences { get; set; }
		public DbSet<ExperienceCategory> ExperienceCategories { get; set; }
		public DbSet<ExperienceCompensation> ExperienceCompensations { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<File> Files { get; set; }
		public DbSet<AssetGroup> AssetGroups { get; set; }
		public DbSet<Asset> Assets { get; set; }
		public DbSet<AssetTag> AssetTags { get; set; }
		public DbSet<AssetFile> AssetFiles { get; set; }
		public DbSet<AssetAction> AssetActions { get; set; }
		public DbSet<RoomAssetModel> RoomAssetModels { get; set; }
		public DbSet<AssetModel> AssetModels { get; set; }
		public DbSet<CleaningPlan> CleaningPlans { get; set; }
		public DbSet<CleaningPlanCpsatConfiguration> CleaningPlanCpsatConfigurations { get; set; }
		public DbSet<CleaningPlanItem> CleaningPlanItems { get; set; }
		public DbSet<PlannableCleaningPlanItem> PlannableCleaningPlanItems { get; set; }
		public DbSet<CleaningPlanGroup> CleaningPlanGroups { get; set; }
		public DbSet<CleaningPlanGroupAvailabilityInterval> CleaningPlanGroupAvailabilityIntervals { get; set; }
		//public DbSet<CleaningPlanGroupFloorAffinity> CleaningPlanGroupFloorAffinities { get; set; }
		public DbSet<CleaningPlanGroupAffinity> CleaningPlanGroupAffinities { get; set; }

		public DbSet<SystemTask> SystemTasks { get; set; }
		public DbSet<SystemTaskAction> SystemTaskActions { get; set; }
		public DbSet<SystemTaskConfiguration> SystemTaskConfigurations { get; set; }
		public DbSet<SystemTaskHistory> SystemTaskHistorys { get; set; }
		public DbSet<SystemTaskMessage> SystemTaskMessages { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<ApplicationUserAvatar> ApplicationUserAvatar { get; set; }
		public DbSet<CleaningPlugin> CleaningPlugins { get; set; }
		public DbSet<Warehouse> Warehouses { get; set; }

		public DbSet<LostAndFound> LostAndFounds { get; set; }
		public DbSet<LostAndFoundFile> LostAndFoundFiles { get; set; }
		public DbSet<OnGuard> OnGuards { get; set; }
		public DbSet<OnGuardFile> OnGuardFiles { get; set; }

		//public DbSet<User> Users { get; set; }
		//public DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
		//public DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }
		//public DbSet<Role> Roles { get; set; }



		public DbSet<Inventory> Inventories { get; set; }
		public DbSet<InventoryAssetStatus> InventoryAssetStatuses { get; set; }
		public DbSet<RoomAssetUsage> RoomAssetUsages { get; set; }
		public DbSet<WarehouseAssetAvailability> WarehouseAssetAvailabilities { get; set; }
		public DbSet<WarehouseDocument> WarehouseDocuments { get; set; }
		public DbSet<WarehouseDocumentArchive> WarehouseDocumentArchives { get; set; }


		public DbSet<CleaningInspection> CleaningInspections { get; set; }
		//public DbSet<CleaningPlanItemEvent> CleaningPlanItemEvents { get; set; }
		public DbSet<CleaningPlanSendingHistory> CleaningPlanSendingHistories { get; set; }
		public DbSet<AutomaticHousekeepingUpdateCycle> HousekeepingNightlyUpdateCycles { get; set; }
		public DbSet<AutomaticHousekeepingUpdateSettings> AutomaticHousekeepingUpdateSettingss { get; set; }
		public DbSet<RccProduct> RccProducts { get; set; }

		public DbSet<RoomNote> RoomNotes { get; set; }
		public DbSet<RoomBed> RoomBeds { get; set; }
		public DbSet<Cleaning> Cleanings { get; set; }
		//public DbSet<UserStatus> UserStatuses { get; set; }
		//public DbSet<UserStatusHistory> UserStatusHistories { get; set; }

		public DbSet<RoomHistoryEvent> RoomHistoryEvents { get; set; }
		public DbSet<UserHistoryEvent> UserHistoryEvents { get; set; }
		public DbSet<CleaningHistoryEvent> CleaningHistoryEvents { get; set; }
		public DbSet<RccHousekeepingStatusColor> RccHousekeepingStatusColors { get; set; }
		//public DbSet<RoomBed> RoomBeds { get; set; }

		public DbSet<RoomMessage> RoomMessages { get; set; }
		public DbSet<RoomMessageFilter> RoomMessageFilters { get; set; }
		public DbSet<RoomMessageDate> RoomMessageDates { get; set; }
		public DbSet<RoomMessageRoom> RoomMessageRooms { get; set; }
		public DbSet<RoomMessageReservation> RoomMessageReservations { get; set; }
		public DbSet<CleaningGeneratorLog> CleaningGeneratorLogs { get; set; }

		public HotelGroupTenantData HotelGroupTenant { get; private set; }

		#region Context configuration
		private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;
		private readonly IHotelGroupTenantProvider _tenantProvider;
		public DatabaseContext(DbContextOptions<DatabaseContext> options,
			IOptions<OperationalStoreOptions> operationalStoreOptions,
			IHotelGroupTenantProvider tenantProvider
			) : base(options)
		{
			this._operationalStoreOptions = operationalStoreOptions;
			this._tenantProvider = tenantProvider;
			this.HotelGroupTenant = tenantProvider?.GetTenant();
		}

		Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (this.HotelGroupTenant != null && this.HotelGroupTenant.ConnectionString.IsNotNull())
			{
				optionsBuilder.UseNpgsql(this.HotelGroupTenant.ConnectionString);
			}

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);

			modelBuilder.UseSerialColumns();

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly, (Type type) =>
			{
				return !type.Namespace.Contains("MasterConfigurations");
			});
			//modelBuilder.ApplyAllConfigurations();
			// REMOVE ALL CASCADE DELETES - In case of deletion developer must manually delete all referenced data.
			//foreach (var relationship in modelBuilder.Model.GetEntityTypes().Where(e => !e.IsOwned()).SelectMany(e => e.GetForeignKeys()))
			//{
			//	relationship.DeleteBehavior = DeleteBehavior.Restrict;
			//}

			foreach (var entity in modelBuilder.Model.GetEntityTypes())
			{
				var entityTableName = entity.GetTableName();
				var entityTableNameSnakeCase = entityTableName.ToSnakeCase();
				entity.SetTableName(entityTableNameSnakeCase);

				foreach (var property in entity.GetProperties())
				{
					property.SetColumnName(property.GetColumnName().ToSnakeCase());
				}

				foreach (var key in entity.GetKeys())
				{
					key.SetName(key.GetName().ToSnakeCase());
				}

				foreach (var key in entity.GetForeignKeys())
				{
					key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
				}

				foreach (var index in entity.GetIndexes())
				{
					index.SetName(index.GetName().ToSnakeCase());
				}
			}

			modelBuilder
				.Entity<User>()
				.Property(u => u.DefaultAvatarColorHex)
				.HasColumnName(nameof(User.DefaultAvatarColorHex))
				.IsRequired()
				.HasDefaultValue("#EEEEEE");

			modelBuilder
				.Entity<User>()
				.HasMany(e => e.UserRoles)
				.WithOne()
				.HasForeignKey(e => e.UserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder
				.Entity<User>()
				.HasMany(e => e.UserClaims)
				.WithOne()
				.HasForeignKey(e => e.UserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);
		}

		public void SetTenantId(Guid hotelGroupId)
		{
			this._tenantProvider.SetTenantId(hotelGroupId);
			this.HotelGroupTenant = this._tenantProvider.GetTenant();
			this.Database.SetConnectionString(this.HotelGroupTenant.ConnectionString);
		}
		public Guid GetTenantId(string hotelGroupKey)
		{
			return this._tenantProvider.GetTenant(hotelGroupKey)?.Id ?? Guid.Empty;
		}

		public bool SetTenantKey(string key)
		{
			this._tenantProvider.SetTenantKey(key);

			var tenant = this._tenantProvider.GetTenant();

			if(tenant == null)
			{
				return false;
			}

			this.HotelGroupTenant = tenant;
			this.Database.SetConnectionString(this.HotelGroupTenant.ConnectionString);
			return true;
		}

		public bool DoesHotelGroupExist(Guid hotelGroupId)
		{
			return this._tenantProvider.CheckIfTenantIdExists(hotelGroupId);
		}
		public bool DoesHotelGroupExist(string hotelGroupKey)
		{
			return this._tenantProvider.CheckIfTenantKeyExists(hotelGroupKey);
		}
		#endregion
	}
}
