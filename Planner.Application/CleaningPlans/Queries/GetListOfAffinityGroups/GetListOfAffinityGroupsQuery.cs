using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Queries.GetListOfAffinityGroups
{
	public class AffinityGroup
	{
		public CleaningPlanGroupAffinityType AffinityType { get; set; }
		public string AffinityGroupName { get; set; }
		public IEnumerable<AffinityData> Affinities { get; set; }
	}
	public class AffinityData
	{
		public string ReferenceId { get; set; }
		public string ReferenceName { get; set; }
		public string ReferenceDescription { get; set; }
	}

	public class GetListOfAffinityGroupsQuery : IRequest<IEnumerable<AffinityGroup>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfAffinityGroupsQueryHandler : IRequestHandler<GetListOfAffinityGroupsQuery, IEnumerable<AffinityGroup>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetListOfAffinityGroupsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<AffinityGroup>> Handle(GetListOfAffinityGroupsQuery request, CancellationToken cancellationToken)
		{
			var buildingAffinities = await this._databaseContext
				.Buildings
				.Where(b => b.HotelId == request.HotelId)
				.Select(b => new AffinityData
				{
					ReferenceId = b.Id.ToString(),
					ReferenceName = b.Name,
					ReferenceDescription = "Building"
				})
				.ToArrayAsync();

			var floorAffinities = await this._databaseContext
				.Floors
				.Where(f => f.Building.HotelId == request.HotelId)
				.OrderBy(f => f.Number)
				.Select(f => new AffinityData
				{
					ReferenceId = f.Id.ToString(),
					ReferenceName = f.Name,
					ReferenceDescription = "Floor in " +f.Building.Name
				})
				.ToArrayAsync();

			var floorSectionsAndSubsections = await this._databaseContext
				.Rooms
				.Where(r => r.HotelId == request.HotelId && r.FloorId != null)
				.Select(r => new
				{
					FloorName = r.Floor.Name,
					r.FloorId,
					r.FloorSectionName,
					r.FloorSubSectionName
				})
				.Where(r => (r.FloorSectionName != null && r.FloorSectionName != "") || (r.FloorSubSectionName != null && r.FloorSubSectionName != ""))
				.ToArrayAsync();

			var floorSectionAffinities = new List<AffinityData>();
			var floorSubSectionAffinities = new List<AffinityData>();

			var floorsMap = new Dictionary<Guid, Dictionary<string, HashSet<string>>>();
			foreach(var ss in floorSectionsAndSubsections)
			{
				var floorSectionsMap = new Dictionary<string, HashSet<string>>();
				if (floorsMap.ContainsKey(ss.FloorId.Value))
				{
					floorSectionsMap = floorsMap[ss.FloorId.Value];
				}
				else
				{
					floorsMap.Add(ss.FloorId.Value, floorSectionsMap);
				}

				if (ss.FloorSectionName.IsNotNull())
				{
					if (!floorSectionsMap.ContainsKey(ss.FloorSectionName))
					{
						floorSectionsMap.Add(ss.FloorSectionName, new HashSet<string>());

						floorSectionAffinities.Add(new AffinityData
						{
							ReferenceId = $"{ss.FloorId.Value.ToString()}|{ss.FloorSectionName}",
							ReferenceName = ss.FloorSectionName,
							ReferenceDescription = $"Section of {ss.FloorName}"
						});
					}

					if (ss.FloorSubSectionName.IsNotNull())
					{
						if (!floorSectionsMap[ss.FloorSectionName].Contains(ss.FloorSubSectionName))
						{
							floorSectionsMap[ss.FloorSectionName].Add(ss.FloorSubSectionName);

							floorSubSectionAffinities.Add(new AffinityData
							{
								ReferenceId = $"{ss.FloorId.Value.ToString()}|{ss.FloorSectionName}|{ss.FloorSubSectionName}",
								ReferenceName = ss.FloorSubSectionName,
								ReferenceDescription = $"Subsection of {ss.FloorSectionName} of {ss.FloorName}"
							});
						}
					}
				}
			}

			var affinityGroups = new List<AffinityGroup>();

			if (buildingAffinities.Any())
			{
				affinityGroups.Add(new AffinityGroup
				{
					Affinities = buildingAffinities,
					AffinityGroupName = "Buildings",
					AffinityType = CleaningPlanGroupAffinityType.BUILDING,
				});
			}

			if (floorAffinities.Any())
			{
				affinityGroups.Add(new AffinityGroup
				{
					Affinities = floorAffinities,
					AffinityGroupName = "Floors",
					AffinityType = CleaningPlanGroupAffinityType.FLOOR,
				});
			}

			if (floorSectionAffinities.Any())
			{
				affinityGroups.Add(new AffinityGroup
				{
					Affinities = floorSectionAffinities,
					AffinityGroupName = "Floor sections",
					AffinityType = CleaningPlanGroupAffinityType.FLOOR_SECTION,
				});
			}

			if (floorSubSectionAffinities.Any())
			{
				affinityGroups.Add(new AffinityGroup
				{
					Affinities = floorSubSectionAffinities,
					AffinityGroupName = "Floor subsections",
					AffinityType = CleaningPlanGroupAffinityType.FLOOR_SUB_SECTION,
				});
			}

			return affinityGroups;
		}
	}
}
