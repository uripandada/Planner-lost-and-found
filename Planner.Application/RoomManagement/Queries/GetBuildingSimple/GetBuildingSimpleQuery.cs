using MediatR;
using Planner.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Queries.GetBuildingSimple
{
	public class BuildingSimpleData
	{
		public Guid Id { get; set; }
		public string TypeKey { get; set; }
	}

	public class GetBuildingSimpleQuery : IRequest<BuildingSimpleData>
	{
		public Guid Id { get; set; }
	}

	public class GetBuildingSimpleQueryHandler : IRequestHandler<GetBuildingSimpleQuery, BuildingSimpleData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetBuildingSimpleQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<BuildingSimpleData> Handle(GetBuildingSimpleQuery request, CancellationToken cancellationToken)
		{
			var building = await this._databaseContext.Buildings.FindAsync(request.Id);

			return new BuildingSimpleData
			{
				Id = building.Id,
				TypeKey = building.TypeKey
			};
		}
	}
}
