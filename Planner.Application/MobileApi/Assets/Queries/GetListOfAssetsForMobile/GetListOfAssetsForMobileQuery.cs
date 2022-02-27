using MediatR;
using Microsoft.AspNetCore.Http;
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

namespace Planner.Application.MobileApi.Assets.Queries.GetListOfAssetsForMobile
{
	public class MobileAsset
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		/// <summary>
		/// SIMPLE, GROUP
		/// </summary>
		public string TypeKey { get; set; }
		public Guid? AssetGroupId { get; set; }
		public string AssetGroupName { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public IEnumerable<string> Tags { get; set; }
	}

	public class GetListOfAssetsForMobileQuery : IRequest<IEnumerable<MobileAsset>>
	{

	}

	public class GetListOfAssetsForMobileQueryHandler : IRequestHandler<GetListOfAssetsForMobileQuery, IEnumerable<MobileAsset>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly IFileService _fileService;

		public GetListOfAssetsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._fileService = fileService;
		}

		public async Task<IEnumerable<MobileAsset>> Handle(GetListOfAssetsForMobileQuery request, CancellationToken cancellationToken)
		{
			var assetGroups = await this._databaseContext
				.AssetGroups
				.ToListAsync();

			var assets = (await this._databaseContext
				.Assets
				.ToListAsync())
				.GroupBy(aa => aa.AssetGroupId)
				.ToDictionary(group => group.Key, group => group.ToArray());

			var assetTags = (await this._databaseContext
				.AssetTags
				.ToListAsync())
				.GroupBy(aa => aa.AssetId)
				.ToDictionary(group => group.Key, group => group.Select(g => g.TagKey).ToArray());

			var result = new List<MobileAsset>();
			foreach (var group in assetGroups)
			{
				var groupAssets = assets.ContainsKey(group.Id) ? assets[group.Id] : new Domain.Entities.Asset[0];

				foreach(var asset in groupAssets)
				{
					result.Add(new MobileAsset
					{
						AssetGroupId = group.Id,
						AssetGroupName = group.Name,
						TypeKey = group.TypeKey,

						Id = asset.Id,
						Name = asset.Name,
						IsBulk = asset.IsBulk,
						SerialNumber = asset.SerialNumber,
						ImageUrl = null,

						Tags = assetTags.ContainsKey(asset.Id) ? assetTags[asset.Id] : new string[0],
					});
				}
			}

			return result;
		}
	}
}
