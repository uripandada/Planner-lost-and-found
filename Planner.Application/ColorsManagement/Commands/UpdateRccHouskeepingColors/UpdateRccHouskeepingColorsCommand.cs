using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Data;
using Planner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ColorsManagement.Commands.UpdateRccHouskeepingColors
{
	public class SaveRccHousekeepingStatusColor
	{
		public RccHousekeepingStatusCode RccCode { get; set; }
		public string ColorHex { get; set; }
	}

	public class UpdateRccHouskeepingColorsCommand : IRequest<ProcessResponse>
	{
		public IEnumerable<SaveRccHousekeepingStatusColor> Colors { get; set; }
	}

	public class UpdateRccHouskeepingColorsCommandHandler : IRequestHandler<UpdateRccHouskeepingColorsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UpdateRccHouskeepingColorsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(UpdateRccHouskeepingColorsCommand request, CancellationToken cancellationToken)
		{
			var colors = await this._databaseContext.RccHousekeepingStatusColors.ToDictionaryAsync(c => c.RccCode);

			var dataChanged = false;
			var colorsToInsert = new List<Domain.Entities.RccHousekeepingStatusColor>();

			foreach (var colorData in request.Colors)
			{
				var color = (Domain.Entities.RccHousekeepingStatusColor)null;
				if (colors.ContainsKey(colorData.RccCode))
				{
					color = colors[colorData.RccCode];

					color.ColorHex = colorData.ColorHex;
					dataChanged = true;
				}
				else
				{
					colorsToInsert.Add(new Domain.Entities.RccHousekeepingStatusColor
					{
						ColorHex = colorData.ColorHex,
						RccCode = colorData.RccCode,
					});
					dataChanged = true;
				}
			}

			if (colorsToInsert.Any())
			{
				await this._databaseContext.RccHousekeepingStatusColors.AddRangeAsync(colorsToInsert);
			}

			if (dataChanged)
			{
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Colors updated"
			};
		}
	}
}
