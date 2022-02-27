using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExternalApi.LostAndFound.Queries
{
	public class ExternalLostItem
	{

	}

	public class ExternalFoundItem
	{

	}

	public class GetListOfLostsQuery: IRequest<IEnumerable<ExternalLostItem>>
	{
		public Guid? HotelGroupId { get; set; }
		public string HotelGroupKey { get; set; }
		public string HotelName { get; set; }
		public string HotelId { get; set; }
	}

	public class GetListOfLostsQueryHandler : IRequestHandler<GetListOfLostsQuery, IEnumerable<ExternalLostItem>>, IAmWebApplicationHandler
	{
		private IMasterDatabaseContext _masterDatabaseContext;
		private IDatabaseContext _databaseContext;
		private IHttpContextAccessor _contextAccessor;

		public GetListOfLostsQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._databaseContext = databaseContext;
			this._contextAccessor = contextAccessor;
		}

		public async Task<IEnumerable<ExternalLostItem>> Handle(GetListOfLostsQuery request, CancellationToken cancellationToken)
		{
			var externalClientAuthenticator = new ExternalClientAuthenticator();
			var authResult = await externalClientAuthenticator.AuthenticateUser(this._masterDatabaseContext, this._contextAccessor.HttpContext.Request.Headers);

			if (authResult.HasError)
			{
				return new ExternalLostItem[0];
			}

			if (!this._SetDatabaseTenant(this._databaseContext, request.HotelGroupId, request.HotelGroupKey))
			{
				return new ExternalLostItem[0];
			}

			var lostItems = await this._databaseContext
				.LostAndFounds
				.Where(lf => lf.HotelId == request.HotelId && lf.Type == Domain.Values.LostAndFoundRecordType.Lost)
				.Select(lf => new ExternalLostItem 
				{ 
				
				})
				.ToListAsync();

			return lostItems;
		}

		private bool _SetDatabaseTenant(IDatabaseContext databaseContext, Guid? hotelGroupId, string hotelGroupKey)
		{
			if (hotelGroupId.HasValue)
			{
				if (this._databaseContext.DoesHotelGroupExist(hotelGroupId.Value))
				{
					this._databaseContext.SetTenantId(hotelGroupId.Value);
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (hotelGroupKey.IsNotNull())
			{
				if (this._databaseContext.DoesHotelGroupExist(hotelGroupKey))
				{
					var tenantId = this._databaseContext.GetTenantId(hotelGroupKey);
					this._databaseContext.SetTenantId(tenantId);
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}

	public class GetListOfFoundsQuery : IRequest<IEnumerable<ExternalFoundItem>>
	{
		public Guid? HotelGroupId { get; set; }
		public string HotelGroupKey { get; set; }
		public string HotelName { get; set; }
		public string HotelId { get; set; }
	}

	public class GetListOfFoundsQueryHandler : IRequestHandler<GetListOfFoundsQuery, IEnumerable<ExternalFoundItem>>, IAmWebApplicationHandler
	{
		private IMasterDatabaseContext _masterDatabaseContext;
		private IDatabaseContext _databaseContext;
		private IHttpContextAccessor _contextAccessor;

		public GetListOfFoundsQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._databaseContext = databaseContext;
			this._contextAccessor = contextAccessor;
		}

		public async Task<IEnumerable<ExternalFoundItem>> Handle(GetListOfFoundsQuery request, CancellationToken cancellationToken)
		{
			var externalClientAuthenticator = new ExternalClientAuthenticator();
			var authResult = await externalClientAuthenticator.AuthenticateUser(this._masterDatabaseContext, this._contextAccessor.HttpContext.Request.Headers);

			if (authResult.HasError)
			{
				return new ExternalFoundItem[0];
			}

			if (!this._SetDatabaseTenant(this._databaseContext, request.HotelGroupId, request.HotelGroupKey))
			{
				return new ExternalFoundItem[0];
			}

			var foundItems = await this._databaseContext
				.LostAndFounds
				.Where(lf => lf.HotelId == request.HotelId && lf.Type == Domain.Values.LostAndFoundRecordType.Found)
				.Select(lf => new ExternalFoundItem
				{

				})
				.ToListAsync();

			return foundItems;
		}

		private bool _SetDatabaseTenant(IDatabaseContext databaseContext, Guid? hotelGroupId, string hotelGroupKey)
		{
			if (hotelGroupId.HasValue)
			{
				if (this._databaseContext.DoesHotelGroupExist(hotelGroupId.Value))
				{
					this._databaseContext.SetTenantId(hotelGroupId.Value);
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (hotelGroupKey.IsNotNull())
			{
				if (this._databaseContext.DoesHotelGroupExist(hotelGroupKey))
				{
					var tenantId = this._databaseContext.GetTenantId(hotelGroupKey);
					this._databaseContext.SetTenantId(tenantId);
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
