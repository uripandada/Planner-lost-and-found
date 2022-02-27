using MediatR;
using Planner.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningPlugins.Queries.GetCleaningPluginsConfigurationData
{


	public class CleaningPluginsConfigurationData
	{
		public CleaningPluginTypeData[] CleaningPluginTypes { get; set; }
		public CleaningPluginBasedOnConfigurationData[] CleaningPluginBasedOns { get; set; }
		public CleaningPluginDisplayStyleData[] DisplayStyles { get; set; }

		public CleaningPluginBasedOnData[] SelectedBasedOns { get; set; }

	}

	public class CleaningPluginTypeData
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public string ParametersDescription { get; set; }
		public string ParametersExpectedFormat { get; set; }
	}

	public class CleaningPluginBasedOnConfigurationData
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public string ParametersDescription { get; set; }
		public string PredefinedParameters { get; set; }

	}

	//public class CleaningPluginBasedOnOccupation
	//{

	//}
	//public class CleaningPluginBasedOnBuilding
	//{

	//}
	//public class CleaningPluginBasedOnRoom
	//{

	//}
	//public class CleaningPluginBasedOnFloorSection
	//{

	//}
	//public class CleaningPluginBasedOnFloorSubSection
	//{

	//}

	public class CleaningPluginBasedOnData : CleaningPluginBasedOnConfigurationData
	{
		public Guid Id { get; set; }
	}

	public class CleaningPluginDisplayStyleData
	{
		public string Name { get; set; }
	}

	public class GetCleaningPluginsConfigurationDataQuery : IRequest<CleaningPluginsConfigurationData>
	{
	}

	public class GetCleaningPluginsConfigurationDataQueryHandler : IRequestHandler<GetCleaningPluginsConfigurationDataQuery, CleaningPluginsConfigurationData>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public GetCleaningPluginsConfigurationDataQueryHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<CleaningPluginsConfigurationData> Handle(GetCleaningPluginsConfigurationDataQuery request, CancellationToken cancellationToken)
		{
			return new CleaningPluginsConfigurationData
			{
				DisplayStyles = new CleaningPluginDisplayStyleData[]
				{
					new CleaningPluginDisplayStyleData { Name = "Bar" }
				},

				CleaningPluginTypes = new CleaningPluginTypeData[]
				{
					new CleaningPluginTypeData { Key = "DAILY", Name = "Every day cleaning", ParametersDescription = "Cleaning Day and Hours", ParametersExpectedFormat = "Every Day,Monday,Tuesday,12=00,14=00,15=00" },
					new CleaningPluginTypeData { Key = "WEEKLY", Name = "Specific day of week cleaning", ParametersDescription = "Cleaning Day", ParametersExpectedFormat = "This Plugin ...." },
					//new CleaningPluginTypeData { Key = "BALANCED_PERIODICAL", Name = "Balanced Periodical Cleaning", ParametersDescription = "Number Of Cleanings", ParametersExpectedFormat = "1,2,3,Automatic" },
					//new CleaningPluginTypeData { Key = "BALANCED", Name = "Balanced Cleaning", ParametersDescription = "Number Of Cleanings", ParametersExpectedFormat = "1,2,3,Automatic" },
					new CleaningPluginTypeData { Key = "WEEK_BASED", Name = "Week based Cleaning", ParametersDescription = "Cleanings Weeks", ParametersExpectedFormat = "W1,W2,W3" },
					new CleaningPluginTypeData { Key = "PERIODICAL", Name = "Periodical cleaning", ParametersDescription = "", ParametersExpectedFormat = "" },
					new CleaningPluginTypeData { Key = "NO_CLEANING", Name = "No Cleaning", ParametersDescription = "No Cleaning", ParametersExpectedFormat = "NoParameter" },
					new CleaningPluginTypeData { Key = "MONTHLY", Name = "Monthly Cleaning", ParametersDescription = "Monthly Cleaning", ParametersExpectedFormat = "Begining,Middle,End,Slice" },

				},
				CleaningPluginBasedOns = new CleaningPluginBasedOnConfigurationData[]
				{
					new CleaningPluginBasedOnConfigurationData { Key = "ALL", Name = "All", ParametersDescription = "NoParameter", PredefinedParameters = "" },
					new CleaningPluginBasedOnConfigurationData { Key = "OCCUPATION", Name = "Occupation", ParametersDescription = "Occupation", PredefinedParameters = "DEP,VAC,STAY,OOS" },
					new CleaningPluginBasedOnConfigurationData { Key = "ROOM", Name = "Room Name", ParametersDescription = "Room Name", PredefinedParameters = "STX,DLX,QUE" },
					new CleaningPluginBasedOnConfigurationData { Key = "ROOM_CATEGORY", Name = "Room Category", ParametersDescription = "Room Category", PredefinedParameters = "STX,DLX,QUE" },
					new CleaningPluginBasedOnConfigurationData { Key = "NIGHTS", Name = "Nights", ParametersDescription = "Number of Nights", PredefinedParameters = "1,2,3,4,5,6,7,8,9,10" },
					new CleaningPluginBasedOnConfigurationData { Key = "RESERVATION_SPACE_CATEGORY", Name = "Reservation Space Category", ParametersDescription = "Reservation Space Category", PredefinedParameters = "Extended Stay" },
					new CleaningPluginBasedOnConfigurationData { Key = "PRODUCT_TAG", Name = "Product/Tag", ParametersDescription = "Product/Tag", PredefinedParameters = "Hebdo,Lundi,Pas de menage" },
					new CleaningPluginBasedOnConfigurationData { Key = "OTHER_PROPERTIES", Name = "Other properties", ParametersDescription = "Product/Tag", PredefinedParameters = "Hebdo,Lundi,Pas de menage" },
					new CleaningPluginBasedOnConfigurationData { Key = "FLOOR", Name = "Floor", ParametersDescription = "Floors", PredefinedParameters = "1,2,3,4,5" },
					new CleaningPluginBasedOnConfigurationData { Key = "SECTION", Name = "Section", ParametersDescription = "Sections", PredefinedParameters = "1B,5B" },
					new CleaningPluginBasedOnConfigurationData { Key = "SUB_SECTION", Name = "SubSection", ParametersDescription = "SubSections", PredefinedParameters = "3A,3B" },
					new CleaningPluginBasedOnConfigurationData { Key = "CLEANLINESS", Name = "Clean & Dirty", ParametersDescription = "Clean & Dirty", PredefinedParameters = "Only clean rooms, only dirty rooms" },
				},
			};
		}
	}
}
