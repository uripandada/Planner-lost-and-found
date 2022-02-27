using Microsoft.AspNetCore.Mvc;
using Planner.Application.Admin.CleaningPlugins.Commands.DeleteCleaningPlugin;
using Planner.Application.Admin.CleaningPlugins.Commands.InsertCleaningPlugin;
using Planner.Application.Admin.CleaningPlugins.Commands.UpdateCleaningPlugin;
using Planner.Application.Admin.CleaningPlugins.Commands.UpdatePluginOrder;
using Planner.Application.Admin.CleaningPlugins.Queries.GetCleaningPluginDetails;
using Planner.Application.Admin.CleaningPlugins.Queries.GetCleaningPluginsConfigurationData;
using Planner.Application.Admin.CleaningPlugins.Queries.GetHotelCleaningPlugins;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Controllers
{
	public class CleaningPluginsController : BaseController
	{
		[HttpPost]
		public async Task<CleaningPluginsConfigurationData> GetCleaningPluginsConfiguration(GetCleaningPluginsConfigurationDataQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<CleaningPluginGridData>> GetHotelCleaningPlugins(GetHotelCleaningPluginsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<CleaningPluginDetailsData> GetCleaningPluginDetails(GetCleaningPluginDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertCleaningPlugin(InsertCleaningPluginCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> UpdateCleaningPlugin(UpdateCleaningPluginCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> DeleteCleaningPlugin(DeleteCleaningPluginCommand request)
		{
			return await this.Mediator.Send(request);
		}


		[HttpPost]
		public async Task<ProcessResponse> UpdatePluginOrder(UpdatePluginOrderCommand request)
		{
			return await this.Mediator.Send(request);
		}


		[HttpPost]
		public async Task<Application.Admin.CleaningCalendar.Queries.GetWeeklyCleaningCalendar.CleaningPredictionsTester.PredictionsTesterResult> GetCleaningPluginTests(Application.Admin.CleaningCalendar.Queries.GetWeeklyCleaningCalendar.CleaningPredictionsTester.PredictionsTesterRequest request)
		{
			var cleaningDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

			var tester = new Application.Admin.CleaningCalendar.Queries.GetWeeklyCleaningCalendar.CleaningPredictionsTester();
			var result = tester.CalculateCleanings(cleaningDate, request.ReservationsKey, request.BasedOnKey);

			return result;
		}
	}
}
