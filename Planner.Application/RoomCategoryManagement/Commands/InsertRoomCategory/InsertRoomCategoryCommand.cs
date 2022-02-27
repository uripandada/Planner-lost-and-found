using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomCategoryManagement.Commands.InsertRoomCategory
{
	public class InsertRoomCategoryCommand : IRequest<ProcessResponse<Guid>>
	{
		public string Name { get; set; }
		public bool IsPrivate { get; set; }
		public bool IsDefaultForReservationSync { get; set; }
	}

	public class InsertRoomCategoryCommandHandler : IRequestHandler<InsertRoomCategoryCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertRoomCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertRoomCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = new RoomCategory
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Id = Guid.NewGuid(),
				IsPrivate = request.IsPrivate,
				Name = request.Name,
				IsDefaultForReservationSync = request.IsDefaultForReservationSync,
				IsSystemDefaultForReservationSync = false,
			};

			await this._databaseContext.RoomCategorys.AddAsync(category);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = category.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Room category inserted."
			};
		}
	}

	public class InsertRoomCategoryCommandValidator : AbstractValidator<InsertRoomCategoryCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public InsertRoomCategoryCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var category = await this._databaseContext.RoomCategorys.Where(t => t.Name.ToLower() == key.ToLower()).FirstOrDefaultAsync();
				return category == null;
			}).WithMessage("ROOM_CATEGORY_ALREADY_EXISTS");
		}
	}
}
