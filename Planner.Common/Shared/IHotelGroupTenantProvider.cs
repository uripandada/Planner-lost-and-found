using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Common.Shared
{
	public class HotelGroupTenantData
	{
		public Guid Id { get; set; } // 32 bit guid
		public string Key { get; set; } // "hotel-royal"
		public string Name { get; set; } // "Hotel Royal"
		public string ConnectionString { get; set; } // PostgreSQL connection string to the tenant
		public bool IsActive { get; set; }
	}
	public interface IHotelGroupTenantProvider
	{
		HotelGroupTenantData GetTenant(); 
		HotelGroupTenantData GetTenant(string tenantKey); 
		HotelGroupTenantData GetTenant(Guid tenantId); 
		void SetTenantId(Guid hotelGroupId);
		void SetTenantKey(string tenantKey);
		bool CheckIfTenantKeyExists(string key);
		bool CheckIfTenantIdExists(Guid id);
	}
}
