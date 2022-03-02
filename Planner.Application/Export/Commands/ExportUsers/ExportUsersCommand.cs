using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Domain.Entities;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Export.Commands.ExportUsers
{
	public class ExcelUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string RoleName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string UserGroup { get; set; }
		public bool IsUserGroupLeader { get; set; }
		public string UserSubGroup { get; set; }
		public bool IsUserSubGroupLeader { get; set; }
		public string AccessibleHotels { get; set; }
	}

	public class ExportUsersCommand : IRequest<byte[]>
	{
	}

	public class ExportUsersCommandHandler : IRequestHandler<ExportUsersCommand, byte[]>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public ExportUsersCommandHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<byte[]> Handle(ExportUsersCommand request, CancellationToken cancellationToken)
		{
			byte[] usersExcel = null;
			var users = await this.databaseContext.Users
				.Include(u => u.UserGroup)
				.Include(u => u.UserSubGroup)
				.Include(u => u.UserRoles)
				.Include(u => u.UserClaims)
				.ToArrayAsync();

			var hotels = await this.databaseContext.Hotels.ToDictionaryAsync(h => h.Id);

			var rolesMap = await this.databaseContext.Roles.ToDictionaryAsync(r => r.Id, r => r);

			if (users != null)
			{
				using (ExcelEngine excelEngine = new ExcelEngine())
				{
					var application = excelEngine.Excel;

					application.DefaultVersion = ExcelVersion.Excel2016;

					var workbook = application.Workbooks.Create(1);
					var worksheet = workbook.Worksheets[0];

					worksheet.Range["A1"].Text = "First Name";
					worksheet.Range["B1"].Text = "Last Name";
					worksheet.Range["C1"].Text = "User Name";
					worksheet.Range["D1"].Text = "Password";
					worksheet.Range["E1"].Text = "Role Name";
					worksheet.Range["F1"].Text = "Email";
					worksheet.Range["G1"].Text = "Phone";
					worksheet.Range["H1"].Text = "User Group";
					worksheet.Range["I1"].Text = "Is User Group Leader";
					worksheet.Range["J1"].Text = "User Sub Group";
					worksheet.Range["K1"].Text = "Is User Sub Group Leader";
					worksheet.Range["L1"].Text = "Accessible Hotels";

					var usersForInsert = users.Select(u =>
					{
						var hotelIds = u.UserClaims.Where(c => c.ClaimType == Domain.Values.ClaimsKeys.HotelId).Select(c => c.ClaimValue).ToArray();
						var hasAccessToAllHotels = hotelIds.Contains("ALL");
						var hotelNames = hotelIds.Where(id => id != "ALL").Select(id => hotels[id].Name).ToArray();

						return this._GenerateUserForExcel(u, rolesMap, hotelNames, hasAccessToAllHotels);
					}).ToArray();
					worksheet.ImportData(usersForInsert, 2, 1, false);

					worksheet.UsedRange.AutofitColumns();
					IListObject table = worksheet.ListObjects.Create("UsersTable", worksheet["A1:K" + (usersForInsert.Count() + 1).ToString()]);
					//Formatting table with a built-in style
					table.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium2;



					using (var mstream = new MemoryStream())
					{
						workbook.SaveAs(mstream, ";");
						usersExcel = mstream.ToArray();
					}
				}
			}

			return usersExcel;
		}

		private ExcelUser _GenerateUserForExcel(User user, Dictionary<Guid, Role> rolesMap, IEnumerable<string> hotelNames, bool hasAccessToAllHotels)
		{
			var userRole = user.UserRoles.FirstOrDefault();
			var roleName = (string)null;

			if (userRole != null)
			{
				roleName = rolesMap[userRole.RoleId].Name;
			}

			return new ExcelUser
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				UserName = user.UserName,
				Password = "",
				RoleName = roleName,
				Email = user.Email,
				Phone = user.PhoneNumber,
				UserGroup = user.UserGroup != null ? user.UserGroup.Name : null,
				IsUserGroupLeader = user.UserGroup != null && user.UserSubGroup == null,
				UserSubGroup = user.UserSubGroup != null ? user.UserSubGroup.Name : null,
				IsUserSubGroupLeader = user.IsSubGroupLeader,
				AccessibleHotels = hasAccessToAllHotels ? "ALL" : string.Join(", ", hotelNames),
			};
		}
	}
}
