using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.InsertBuilding
{
	public class InsertBuildingResponse
	{
		public Guid Id { get; set; }
		public bool WasAreaInserted { get; set; }
		public InsertBuildingAreaResponse Area { get; set; }
	}

	public class InsertBuildingAreaResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class InsertBuildingCommand : IRequest<ProcessResponse<InsertBuildingResponse>>
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public long? Latitude { get; set; }
		public long? Longitude { get; set; }
		public Guid? AreaId { get; set; }
		public string AreaName { get; set; }
		public int OrdinalNumber { get; set; }
		public string TypeKey { get; set; }
		public string HotelId { get; set; }
	}
	public class InsertBuildingCommandHandler : IRequestHandler<InsertBuildingCommand, ProcessResponse<InsertBuildingResponse>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertBuildingCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<InsertBuildingResponse>> Handle(InsertBuildingCommand request, CancellationToken cancellationToken)
		{
			var building = new Building
			{
				Id = Guid.NewGuid(),
				Address = request.Address,
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				HotelId = request.HotelId,
				Latitude = request.Latitude,
				Longitude = request.Longitude,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Name = request.Name,
				AreaId = null,
				OrdinalNumber = request.OrdinalNumber,
				TypeKey = request.TypeKey,
			};

			var response = new InsertBuildingResponse
			{
				Id = building.Id,
				WasAreaInserted = false,
				Area = null
			};

			if (request.AreaId.HasValue)
			{
				building.AreaId = request.AreaId.Value;
			}
			else if (request.AreaName.IsNotNull())
			{
				var areaFilterValue = request.AreaName.ToLower();
				var existingArea = await this._databaseContext.Areas.FirstOrDefaultAsync(a => a.HotelId == request.HotelId && a.Name.ToLower() == areaFilterValue);
				if(existingArea != null)
				{
					building.AreaId = existingArea.Id;
				}
				else
				{
					var newArea = new Area
					{
						Id = Guid.NewGuid(),
						CreatedAt = DateTime.UtcNow,
						CreatedById = this._userId,
						HotelId = request.HotelId,
						ModifiedAt = DateTime.UtcNow,
						ModifiedById = this._userId,
						Name = request.AreaName
					};

					await this._databaseContext.Areas.AddAsync(newArea);

					building.AreaId = newArea.Id;

					response.WasAreaInserted = true;
					response.Area = new InsertBuildingAreaResponse
					{
						Id = newArea.Id,
						Name = newArea.Name,
						CreatedAt = newArea.CreatedAt
					};
				}
			}
			else
			{
				building.AreaId = null;
			}

			await this._databaseContext.Buildings.AddAsync(building);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<InsertBuildingResponse>
			{
				Data = response,
				HasError = false,
				IsSuccess = true,
				Message = "Building inserted."
			};
		}
	}
}
