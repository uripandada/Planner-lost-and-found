using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.Hotel.Queries.GetHotelHierarchy
{
	public class HotelHierarchyData
	{
		public IEnumerable<HotelHierarchyBuildingData> Buildings { get; set; }

	}

	public class HotelHierarchyBuildingData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<HotelHierarchyFloorData> Floors { get; set; }

	}
	public class HotelHierarchyFloorData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<HotelHierarchySectionData> Sections { get; set; }
	}
	public class HotelHierarchySectionData
	{
		public string Name { get; set; }
		public IEnumerable<HotelHierarchySubSectionData> SubSections { get; set; }
	}

	public class HotelHierarchySubSectionData
	{
		public string Name { get; set; }
	}

	public class GetHotelHierarchyQuery : IRequest<HotelHierarchyData>
	{
		public string HotelId { get; set; }
	}

	public class GetHotelHierarchyQueryHandler : IRequestHandler<GetHotelHierarchyQuery, HotelHierarchyData>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetHotelHierarchyQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<HotelHierarchyData> Handle(GetHotelHierarchyQuery request, CancellationToken cancellationToken)
		{
			var sectionsMap = (await this._databaseContext
				.Rooms
				.Where(r => r.HotelId == request.HotelId && r.FloorId != null && r.FloorSectionName != null)
				.Select(r => new { FloorSectionName = r.FloorSectionName, FloorSubSectionName = r.FloorSubSectionName, FloorId = r.FloorId })
				.ToArrayAsync())
				.GroupBy(f => f.FloorId)
				.ToDictionary(f => f.Key, f => f.ToArray());

			var buildings = await this._databaseContext
				.Buildings
				.Where(b => b.HotelId == request.HotelId)
				.Select(b => new { b.Id, b.Name, Floors = b.Floors.Select(f => new { f.Id, f.Name }).ToArray() })
				.ToArrayAsync();

			var buildingsData = new List<HotelHierarchyBuildingData>();
			foreach (var building in buildings)
			{
				var b = new HotelHierarchyBuildingData
				{
					Id = building.Id,
					Name = building.Name,
					Floors = building.Floors.Select(f => new HotelHierarchyFloorData
					{
						Id = f.Id,
						Name = f.Name
					}).ToArray()
				};

				buildingsData.Add(b);

				foreach (var floor in b.Floors)
				{
					var sections = new List<HotelHierarchySectionData>();
					floor.Sections = sections;

					if (!sectionsMap.ContainsKey(floor.Id))
					{
						continue;
					}

					var sectionToSubSectionsMap = new Dictionary<string, HashSet<string>>();
					var data = sectionsMap[floor.Id];
					foreach (var sectionSubSection in data)
					{
						var subSectionsSet = new HashSet<string>();
						if (!sectionToSubSectionsMap.ContainsKey(sectionSubSection.FloorSectionName))
						{
							sectionToSubSectionsMap.Add(sectionSubSection.FloorSectionName, subSectionsSet);
						}
						else
						{
							subSectionsSet = sectionToSubSectionsMap[sectionSubSection.FloorSectionName];
						}

						if (!subSectionsSet.Contains(sectionSubSection.FloorSubSectionName))
						{
							subSectionsSet.Add(sectionSubSection.FloorSubSectionName);
						}
					}

					floor.Sections = sectionToSubSectionsMap.Select(s => new HotelHierarchySectionData
					{
						Name = s.Key,
						SubSections = s.Value.Select(ss => new HotelHierarchySubSectionData 
						{ 
							Name = ss
						}).ToArray()
					});
				}
			}

			return new HotelHierarchyData
			{
				Buildings = buildingsData,
			};
		}
	}
}
