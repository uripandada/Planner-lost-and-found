using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Common.Helpers
{
	public class PostgreSqlConnectionStringData
	{
		public string UserId { get; set; }
		public string Password { get; set; }
		public string Host { get; set; }
		public string Port { get; set; }
		public string Database { get; set; }
		public bool Pooling { get; set; }
	}

	public class ConnectionStringHelper
	{
		private static string USER_ID_PREFIX = "User ID=";
		private static string PASSWORD_PREFIX = "Password=";
		private static string HOST_PREFIX = "Host=";
		private static string PORT_PREFIX = "Port=";
		private static string DATABASE_PREFIX = "Database=";
		private static string POOLING_PREFIX = "Pooling=";

		public static PostgreSqlConnectionStringData ParsePostgreSqlConnectionString(string connectionString)
		{
			//User ID=USER_ID_VALUE;Password=PASSWORD_VALUE;Host=HOST_VALUE;Port=PORT_VALUE;Database=DATABASE_VALUE;Pooling=POOLING_VALUE;
			
			var data = new PostgreSqlConnectionStringData();

			if (connectionString.IsNull())
			{
				return data;
			}

			var parts = connectionString.Split(";", StringSplitOptions.RemoveEmptyEntries);

			foreach(var part in parts)
			{
				if (part.StartsWith(USER_ID_PREFIX))
				{
					data.UserId = part.Substring(USER_ID_PREFIX.Length);
				}
				else if (part.StartsWith(PASSWORD_PREFIX))
				{
					data.Password = part.Substring(PASSWORD_PREFIX.Length);
				}
				else if (part.StartsWith(HOST_PREFIX))
				{
					data.Host = part.Substring(HOST_PREFIX.Length);
				}
				else if (part.StartsWith(PORT_PREFIX))
				{
					data.Port = part.Substring(PORT_PREFIX.Length);
				}
				else if (part.StartsWith(DATABASE_PREFIX))
				{
					data.Database = part.Substring(DATABASE_PREFIX.Length);
				}
				else if (part.StartsWith(POOLING_PREFIX))
				{
					data.Pooling = bool.Parse(part.Substring(POOLING_PREFIX.Length));
				}
			}

			return data;
		}

		public static string GeneratePostgreSqlConnectionString(string userId, string password, string host, string port, string database, bool pooling)
		{
			var connectionString = "";
			
			if (userId.IsNotNull())
			{
				connectionString += $"{USER_ID_PREFIX}{userId};";
			}
			
			if (password.IsNotNull())
			{
				connectionString += $"{PASSWORD_PREFIX}{password};";
			}
			
			if (host.IsNotNull())
			{
				connectionString += $"{HOST_PREFIX}{host};";
			}
			
			if (port.IsNotNull())
			{
				connectionString += $"{PORT_PREFIX}{port};";
			}
			
			if (database.IsNotNull())
			{
				connectionString += $"{DATABASE_PREFIX}{database};";
			}
			
			connectionString += $"{POOLING_PREFIX}{pooling};";

			return connectionString;
		}
	}
}
