using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomCategoryManagement.Commands.UpdateRoomCategory
{
	public class UpdateRoomCategoryCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsPrivate { get; set; }
		public bool IsDefaultForReservationSync { get; set; }
	}

	public class UpdateRoomCategoryCommandHandler : IRequestHandler<UpdateRoomCategoryCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateRoomCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateRoomCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.RoomCategorys.FindAsync(request.Id);

			if (category == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find room category to update."
				};
			}

			if(!category.IsDefaultForReservationSync && request.IsDefaultForReservationSync)
			{
				var defaultCategories = await this._databaseContext.RoomCategorys.Where(rc => rc.IsDefaultForReservationSync).ToArrayAsync();
				foreach(var c in defaultCategories)
				{
					c.IsDefaultForReservationSync = false;
				}
			}

			category.ModifiedAt = DateTime.UtcNow;
			category.ModifiedById = this._userId;
			category.Name = request.Name;
			category.IsPrivate = request.IsPrivate;
			category.IsDefaultForReservationSync = request.IsDefaultForReservationSync;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Room category updated."
			};
		}
	}

	public class UpdateRoomCategoryCommandValidator : AbstractValidator<UpdateRoomCategoryCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public UpdateRoomCategoryCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var category = await this._databaseContext.RoomCategorys.Where(t => t.Name.ToLower() == key.ToLower() && t.Id != command.Id).FirstOrDefaultAsync();
				return category == null;
			}).WithMessage("ROOM_CATEGORY_ALREADY_EXISTS");
		}
	}
}
