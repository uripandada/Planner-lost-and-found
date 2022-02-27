using Planner.Application.Interfaces;
using Planner.Common.Shared;
using Planner.Domain.Entities.Master;
using System;

namespace Planner.Persistence
{
	public class ArrayHotelGroupTenantProvider : IHotelGroupTenantProvider
	{
		private HotelGroupTenantData _tenant;

		public ArrayHotelGroupTenantProvider(HotelGroupTenantData tenant)
		{
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

		public HotelGroupTenantData GetTenant()
		{
			return this._tenant;
		}

		public bool CheckIfTenantKeyExists(string key)
		{
			return true;
		}

		public void SetTenantId(Guid hotelGroupId)
		{
			// This should not be called on anything except the regular tenant provider.
			throw new NotSupportedException();
		}

		public void SetTenantKey(string tenantKey)
		{
			throw new NotImplementedException();
		}

		public bool CheckIfTenantIdExists(Guid id)
		{
			throw new NotImplementedException();
		}

		public HotelGroupTenantData GetTenant(string tenantKey)
		{
			throw new NotImplementedException();
		}

		public HotelGroupTenantData GetTenant(Guid tenantId)
		{
			throw new NotImplementedException();
		}
	}
}
