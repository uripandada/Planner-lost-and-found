using Microsoft.AspNetCore.Http;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using Planner.Common.Shared;
using System;
using System.Linq;

namespace Planner.Persistence
{
	public class HotelGroupTenantProvider : IHotelGroupTenantProvider//, IAdminHotelGroupTenantProvider
	{
		private static HotelGroupTenantData[] _TENANTS;

		private HotelGroupTenantData _tenant;

		public HotelGroupTenantProvider(IHttpContextAccessor httpContextAccessor, IMasterDatabaseContext masterDatabaseContext)
		{
			this._LoadTenants(masterDatabaseContext);

			HotelGroupTenantData tenant = null;
			var hotelGroupId = Guid.Empty;

			if (httpContextAccessor != null)
			{
				var tenantKey = httpContextAccessor.TryGetLoginGroupKey();
				if (!string.IsNullOrWhiteSpace(tenantKey))
				{
					var tenantKeyValue = tenantKey?.Trim()?.ToLower();
					tenant = _TENANTS.FirstOrDefault(t => t.Key == tenantKeyValue);
				}
				else
				{
					hotelGroupId = httpContextAccessor.HotelGroupId();
					if (hotelGroupId != Guid.Empty)
					{
						tenant = _TENANTS.FirstOrDefault(t => t.Id == hotelGroupId);
					}
				}
			}

			if (tenant == null)
			{
				tenant = new HotelGroupTenantData
				{
					Id = Guid.Empty,
					ConnectionString = "",
					IsActive = false,
					Key = "#UNKNOWN-HOTEL-GROUP#",
					Name = "Unknown hotel group"
				};
			}

			this._tenant = tenant;
		}

		private void _LoadTenants(IMasterDatabaseContext masterDatabaseContext)
		{
			_TENANTS = masterDatabaseContext.HotelGroupTenants.Select(t => new HotelGroupTenantData
			{
				ConnectionString = t.ConnectionString,
				Id = t.Id,
				IsActive = t.IsActive,
				Key = t.Key.Trim().ToLower(),
				Name = t.Name,
			}).ToArray();
		}

		public HotelGroupTenantData GetTenant()
		{
			return this._tenant;
		}

		public void SetTenantId(Guid hotelGroupId)
		{
			this._tenant = _TENANTS.FirstOrDefault(t => t.Id == hotelGroupId);
		}

		public void SetTenantKey(string tenantKey)
		{
			var keyValue = tenantKey?.Trim()?.ToLower();
			this._tenant = _TENANTS.FirstOrDefault(t => t.Key == keyValue);
		}

		public bool CheckIfTenantKeyExists(string key)
		{
			var keyValue = key?.Trim()?.ToLower();
			this._tenant = _TENANTS.FirstOrDefault(t => t.Key == keyValue);
			return this._tenant != null;
		}

		public bool CheckIfTenantIdExists(Guid id)
		{
			this._tenant = _TENANTS.FirstOrDefault(t => t.Id == id);
			return this._tenant != null;
		}


		public HotelGroupTenantData GetTenant(string key)
		{
			var keyValue = key?.Trim()?.ToLower();
			return _TENANTS.FirstOrDefault(t => t.Key == keyValue);
		}
		public HotelGroupTenantData GetTenant(Guid id)
		{
			return _TENANTS.FirstOrDefault(t => t.Id == id);
		}
	}
}
