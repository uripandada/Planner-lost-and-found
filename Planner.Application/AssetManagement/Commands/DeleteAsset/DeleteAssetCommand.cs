using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Commands.DeleteAsset
{
	public class DeleteAssetCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		//private readonly string _hotelId;

		public DeleteAssetCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			//this._hotelId = contextAccessor.HotelId();
		}

		public Task<ProcessResponse> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
