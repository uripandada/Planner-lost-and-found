using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Common
{
	public enum SystemRole
	{
		Admin = 0,
		Tech = 1,
		Cleaner = 2,
		Host = 3,
		Runner = 4,

		ADMINISTRATOR = 5,
		RECEPTIONIST = 6,
		INSPECTOR = 7,
		MAINTENANCE = 8,
		ATTENDANT = 9,
		HOST = 10,
		RUNNER = 11
	}

	public static class SystemDefaults
	{
		public static class Users
		{
			public static readonly string DefaultAdminUserName = "rcadmin";
			public static readonly string DefaultAdminNormalizedUserName = "RCADMIN";
			public static readonly string DefaultAdminPassword = "rcadmintest123123";
		}

		public static class Roles
		{
			//public static readonly SystemDefaultRole Admin = new SystemDefaultRole(SystemRole.Admin, "Admin", "ADMIN");
			//public static readonly SystemDefaultRole Tech = new SystemDefaultRole(SystemRole.Tech, "Tech", "TECH");
			//public static readonly SystemDefaultRole Cleaner = new SystemDefaultRole(SystemRole.Cleaner, "Cleaner", "CLEANER");





			public static readonly SystemDefaultRole Administrator = new SystemDefaultRole(SystemRole.ADMINISTRATOR, "Administrator", nameof(SystemRole.ADMINISTRATOR));
			public static readonly SystemDefaultRole Receptionist = new SystemDefaultRole(SystemRole.RECEPTIONIST, "Receptionist", nameof(SystemRole.RECEPTIONIST));
			public static readonly SystemDefaultRole Inspector = new SystemDefaultRole(SystemRole.INSPECTOR, "Inspector", nameof(SystemRole.INSPECTOR));
			public static readonly SystemDefaultRole Maintenance = new SystemDefaultRole(SystemRole.MAINTENANCE, "Maintenance", nameof(SystemRole.MAINTENANCE));
			public static readonly SystemDefaultRole Attendant = new SystemDefaultRole(SystemRole.ATTENDANT, "Attendant", nameof(SystemRole.ATTENDANT));
			public static readonly SystemDefaultRole Host = new SystemDefaultRole(SystemRole.HOST, "Host", nameof(SystemRole.HOST));
			public static readonly SystemDefaultRole Runner = new SystemDefaultRole(SystemRole.RUNNER, "Runner", nameof(SystemRole.RUNNER));
		}

		public class SystemDefaultRole
		{
			public SystemDefaultRole(SystemRole type, string name, string normalizedName)
			{
				this.Type = type;
				this.Name = name;
				this.NormalizedName = normalizedName;
			}

			public readonly SystemRole Type;
			public readonly string Name;
			public readonly string NormalizedName;
		}
	}
}
