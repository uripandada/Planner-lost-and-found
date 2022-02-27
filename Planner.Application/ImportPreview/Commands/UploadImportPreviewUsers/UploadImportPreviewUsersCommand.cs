using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ImportPreview.Commands.UploadImportPreviewUsers
{
	public class ImportUsersPreview
	{
		public string FileName { get; set; }
		public IEnumerable<ImportUserPreview> Users { get; set; }
		public bool HasError { get; set; }
		public string Message { get; set; }
	}

	public class ImportUserPreview
	{
		public bool AlreadyExists { get; set; }
		public string DefaultAvatarColorHex { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public bool HasError { get; set; }
		public bool IsUserGroupLeader { get; set; }
		public bool IsUserSubGroupLeader { get; set; }
		public string LastName { get; set; }
		public string Message { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }
		public string RoleName { get; set; }
		public string UserGroup { get; set; }
		public string UserName { get; set; }
		public string UserSubGroup { get; set; }
		public string AccessibleHotels { get; set; }
	}
	
	public class UploadImportPreviewUsersCommand : IRequest<ImportUsersPreview>
	{
		public IFormFile File { get; set; }
	}
	
	public class UploadImportPreviewUsersCommandHandler : IRequestHandler<UploadImportPreviewUsersCommand, ImportUsersPreview>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public UploadImportPreviewUsersCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ImportUsersPreview> Handle(UploadImportPreviewUsersCommand request, CancellationToken cancellationToken)
		{
			var importPreview = new ImportUsersPreview();
			importPreview.FileName = request.File.FileName;
			importPreview.HasError = false;
			importPreview.Message = "Data extracted.";
			importPreview.Users = new List<ImportUserPreview>();

			try
			{
				using (ExcelEngine excelEngine = new ExcelEngine())
				{
					IApplication application = excelEngine.Excel;
					application.CSVSeparator = ";";

					application.DefaultVersion = ExcelVersion.Excel2016;

					using (var ms = new MemoryStream())
					{
						await request.File.CopyToAsync(ms);

						ms.Position = 0;

						IWorkbook workbook = application.Workbooks.Open(ms, ExcelOpenType.Automatic);
						IWorksheet worksheet = workbook.Worksheets[0];
						
						if (worksheet.Rows.Length > 0)
						{
							importPreview.Users = this._ExtractUsersFromTheWorksheet(worksheet);
						}
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			await this._ValidateExtractedData(importPreview);

			importPreview.Users = importPreview.Users
				.OrderBy(r => r.HasError ? 0 : 1)
				.ThenBy(r => r.UserName)
				.ThenBy(r => r.FirstName)
				.ThenBy(r => r.LastName)
				.ToArray();

			return importPreview;
		}

		private IEnumerable<ImportUserPreview> _ExtractUsersFromTheWorksheet(IWorksheet worksheet)
		{
			var users = new List<ImportUserPreview>();

			var columnsMap = _GetColumnsFromWorksheet(worksheet.Rows[0]);

			foreach (IRange userRow in worksheet.Rows.Skip(1))
			{
				var user = new ImportUserPreview();
				user.FirstName = Convert.ToString(userRow.Cells[columnsMap["First Name"]].Value);
				user.LastName = Convert.ToString(userRow.Cells[columnsMap["Last Name"]].Value);
				user.UserName = Convert.ToString(userRow.Cells[columnsMap["User Name"]].Value);
				user.Password = Convert.ToString(userRow.Cells[columnsMap["Password"]].Value);
				user.RoleName = Convert.ToString(userRow.Cells[columnsMap["Role Name"]].Value);
				user.Email = Convert.ToString(userRow.Cells[columnsMap["Email"]].Value);
				user.Phone = Convert.ToString(userRow.Cells[columnsMap["Phone"]].Value);
				user.UserGroup = Convert.ToString(userRow.Cells[columnsMap["User Group"]].Value);
				user.IsUserGroupLeader = Convert.ToBoolean(userRow.Cells[columnsMap["Is User Group Leader"]].Value);
				user.UserSubGroup= Convert.ToString(userRow.Cells[columnsMap["User Sub Group"]].Value);
				user.IsUserSubGroupLeader = Convert.ToBoolean(userRow.Cells[columnsMap["Is User Sub Group Leader"]].Value);
				user.AccessibleHotels = Convert.ToString(userRow.Cells[columnsMap["Accessible Hotels"]].Value);

				users.Add(user);
			}

			return users;
		}

		private Dictionary<string, int> _GetColumnsFromWorksheet(IRange range)
		{
			var columns = new Dictionary<string, int>();

			foreach(var col in range.Columns)
			{
				var _columnName = col.Value;
				var _columnIndex = col.Column - 1;
				columns.Add(_columnName, _columnIndex);
			}

			return columns;
		}

		private async Task _ValidateExtractedData(ImportUsersPreview usersPreview)
		{
			var existingUsers = await this._databaseContext
				.Users
				.Select(u => new
				{
					Id = u.Id,
					NormalizedUserName = u.UserName.Trim().ToLower(),
					NormalizedEmail = u.Email.Trim().ToLower(),
				})
				.ToArrayAsync();

			var hotelsMap = await this._databaseContext.Hotels.Select(h => new { Name = h.Name.Trim().ToLower(), Id = h.Id }).ToDictionaryAsync(h => h.Name);
			var rolesMap = (await this._databaseContext.Roles.ToArrayAsync()).ToDictionary(r => r.Id, r => r);
			var normalizedRoleNamesMap = rolesMap.Values.Select(r => new { Name = r.Name.Trim().ToLower(), Id = r.Id, r.IsSystemRole, r.HotelAccessTypeKey }).DistinctBy(r => r.Name).ToDictionary(r => r.Name, r => r);
			//var userGroups = this._databaseContext.UserGroups.ToArray();
			//var userSubGroups = this._databaseContext.UserSubGroups.ToArray();

			foreach (var user in usersPreview.Users)
			{
				var errorMessage = "";
				var hasError = false;

				if (user.FirstName.IsNull())
				{
					hasError = true;
					errorMessage += $"First Name is required. ";
				}
				
				if (user.LastName.IsNull())
				{
					hasError = true;
					errorMessage += $"LastName is required. ";
				}
				
				if (user.UserName.IsNull())
				{
					hasError = true;
					errorMessage += $"UserName is required. ";
				}
				
				var normalizedUserName = user.UserName.Trim().ToLower();
				var existingUser = existingUsers.FirstOrDefault(eu => eu.NormalizedUserName == normalizedUserName);
				if (user.Password.IsNull())
				{
					// Password can be null only for existing users
					if(existingUser == null)
					{
						hasError = true;
						errorMessage += $"Password is required. ";
					}
				}
				else if(user.Password.Length < 4)
				{
					hasError = true;
					errorMessage += "Password must be at least 4 characters. ";
				}
				
				if (user.RoleName.IsNull())
				{
					hasError = true;
					errorMessage += $"RoleName is required. ";
				}

				var normalizedRoleName = user.RoleName.Trim().ToLower();
				if (!normalizedRoleNamesMap.ContainsKey(normalizedRoleName))
				{
					hasError = true;
					errorMessage = $"Unknown role. ";
				}


				//var userNameAlreadyExists = false;

	//            foreach(var existingUser in existingUsers)
				//{
	//                if(existingUser.NormalizedUserName == normalizedUserName)
				//	{
	//                    userNameAlreadyExists = true;
				//	}
				//}

				if (user.Email.IsNotNull())
				{
					var normalizedEmail = user.Email?.Trim().ToLower();
					var emailAlreadyExists = false;

					foreach (var eu in existingUsers)
					{
						if (eu.NormalizedEmail == normalizedEmail)
						{
							if(eu.NormalizedUserName != normalizedUserName)
							{
								emailAlreadyExists = true;
								break;
							}
						}
					}

					if (emailAlreadyExists)
					{
						hasError = true;
						errorMessage += $"E-mail already exists. ";
					}
				}

				if (user.AccessibleHotels.IsNull())
				{
					hasError = true;
					errorMessage += "Must be assigned to at least one hotel. ";
				}
				else
				{
					var hotelNames = user.AccessibleHotels.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(hn => hn.Trim().ToLower()).ToArray();

					if(hotelNames.Length == 0)
					{
						hasError = true;
						errorMessage += "Must be assigned to at least one hotel. ";
					}
					else
					{
						foreach(var hotelName in hotelNames)
						{
							if(hotelName == "all")
							{
								if (!normalizedRoleNamesMap.ContainsKey(normalizedRoleName) || normalizedRoleNamesMap[normalizedRoleName].HotelAccessTypeKey != "ALL")
								{
									hasError = true;
									errorMessage += $"{normalizedRoleName} can't have access to all hotels. ";
									continue;
								}
							}
							else
							{
								if (!hotelsMap.ContainsKey(hotelName))
								{
									hasError = true;
									errorMessage += $"Hotel {hotelName} doesn't exist. ";
									continue;
								}

								if (!normalizedRoleNamesMap.ContainsKey(normalizedRoleName))
								{
									hasError = true;
									errorMessage += $"Unkonwn role {normalizedRoleName}. Can't give access to hotel {hotelName}. ";
									continue;
								}
							}

						}

						if (normalizedRoleNamesMap.ContainsKey(normalizedRoleName))
						{
							var role = normalizedRoleNamesMap[normalizedRoleName]; //.Values.Where(v => v.HotelAccessTypeKey == "SINGLE").ToArray();
							if(role.HotelAccessTypeKey == "SINGLE" && hotelNames.Length > 1)
							{
								hasError = true;
								errorMessage += $"{role.Name} can have access to only one hotel. ";
							}
						}
						else
						{
							hasError = true;
							errorMessage += $"Unknown role {normalizedRoleName}. Can't determine correct number of accessible hotels.";
						}
					}

				}

				//if (userNameAlreadyExists)
				//{
				//    hasError = true;
				//    errorMessage += $"Username already exists. ";
				//}

				user.HasError = hasError;
				user.Message = errorMessage;
			}
		}
	}
}
