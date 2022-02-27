using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Queries.GetAssetTags
{
	public class TagItemData
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class GetAssetTagsQuery: IRequest<IEnumerable<TagItemData>>
	{

	}

	public class GetAssetTagsQueryHandler : IRequestHandler<GetAssetTagsQuery, IEnumerable<TagItemData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetAssetTagsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<TagItemData>> Handle(GetAssetTagsQuery request, CancellationToken cancellationToken)
		{
			return await this._databaseContext.Tags
				.Select(t => new TagItemData { Key = t.Key, Value = t.Value })
				.ToListAsync();
		}
	}
}
