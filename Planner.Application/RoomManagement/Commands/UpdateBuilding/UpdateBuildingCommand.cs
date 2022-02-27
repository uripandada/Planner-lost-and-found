using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.UpdateBuilding
{
	public class UpdateBuildingResponse
	{
		public bool WasAreaInserted { get; set; }
		public UpdateBuildingAreaResponse Area { get; set; }
	}

	public class UpdateBuildingAreaResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class UpdateBuildingCommand : IRequest<ProcessResponse<UpdateBuildingResponse>>
	{
		public Guid Id { get; set; }
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

	public class UpdateBuildingCommandHandler : IRequestHandler<UpdateBuildingCommand, ProcessResponse<UpdateBuildingResponse>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateBuildingCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<UpdateBuildingResponse>> Handle(UpdateBuildingCommand request, CancellationToken cancellationToken)
		{
			var response = new UpdateBuildingResponse();

			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				var building = await this._databaseContext.Buildings.Include(r => r.Rooms).Where(b => b.Id == request.Id).FirstOrDefaultAsync();

				if(building == null)
				{
					return new ProcessResponse<UpdateBuildingResponse>
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to find building to update."
					};
				}

				if(building.Rooms.Any() && request.TypeKey != building.TypeKey)
				{
					return new ProcessResponse<UpdateBuildingResponse>
					{
						HasError = true,
						IsSuccess = false,
						Message = "Building with rooms can't change type."
					};
				}

				if (request.AreaId.HasValue)
				{
					building.AreaId = request.AreaId.Value;
				}
				else if (request.AreaName.IsNotNull())
				{
					var areaFilterValue = request.AreaName.ToLower();
					var existingArea = await this._databaseContext.Areas.FirstOrDefaultAsync(a => a.HotelId == request.HotelId && a.Name.ToLower() == areaFilterValue);
					if (existingArea != null)
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
						response.Area = new UpdateBuildingAreaResponse
						{
							CreatedAt = newArea.CreatedAt,
							Id = newArea.Id,
							Name = newArea.Name
						};
					}
				}
				else
				{
					building.AreaId = null;
				}

				building.Address = request.Address;
				building.Latitude = request.Latitude;
				building.Longitude = request.Longitude;
				building.ModifiedAt = DateTime.UtcNow;
				building.ModifiedById = this._userId;
				building.Name = request.Name;
				building.OrdinalNumber = request.OrdinalNumber;
				building.TypeKey = request.TypeKey;

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync();
			}

			return new ProcessResponse<UpdateBuildingResponse>
			{
				Data = response,
				HasError = false,
				IsSuccess = true,
				Message = "Building updated."
			};
		}
	}
}
