using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Rooms.Commands.ChangeRoomPriorityForMobile
{
    public class ChangeRoomPriorityForMobileCommand: IRequest<ProcessResponseSimple>
    {
        public string HotelId { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? BedId { get; set; }
        public bool IsPriority { get; set; }
    }

    public class ChangeRoomPriorityForMobileCommandHandler : IRequestHandler<ChangeRoomPriorityForMobileCommand, ProcessResponseSimple>, IAmWebApplicationHandler
    {
        private IDatabaseContext _databaseContext;
        private readonly Guid _userId;
        private readonly Guid _hotelGroupId;
        private readonly string _roleName;
        private readonly ISystemEventsService _systemEventsService;

        public ChangeRoomPriorityForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService systemEventsService)
        {
            this._databaseContext = databaseContext;
            this._userId = contextAccessor.UserId();
            this._hotelGroupId = contextAccessor.HotelGroupId();
            this._roleName = contextAccessor.RoleName();
            this._systemEventsService = systemEventsService;
        }

        public async Task<ProcessResponseSimple> Handle(ChangeRoomPriorityForMobileCommand request, CancellationToken cancellationToken)
        {
            var isInspector = this._roleName == SystemDefaults.Roles.Inspector.Name || this._roleName == SystemDefaults.Roles.Administrator.Name;

            if (!isInspector)
            {
                return new ProcessResponseSimple
                {
                    IsSuccess = false,
                    Message = "Only inspector can change room priority",
                };
            }

            var dateProvider = new HotelLocalDateProvider();
            var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);
            var roomId = request.RoomId.HasValue ? request.RoomId.Value : Guid.Empty;

            if (request.BedId.HasValue)
            {
                var bed = await this._databaseContext.RoomBeds.Include(rb => rb.Room).Where(rb => rb.Id == request.BedId.Value).FirstOrDefaultAsync();
                bed.IsCleaningPriority = request.IsPriority;
                roomId = bed.Room.Id;
            }
            else
            {
                var room = await this._databaseContext.Rooms.FindAsync(request.RoomId.Value);
                room.IsCleaningPriority = request.IsPriority;
            }

            await this._UpdateCleaningPlanItemsAndCleaningsPriorities(request.RoomId, request.BedId, request.IsPriority, dateTime);

            await this._databaseContext.SaveChangesAsync(cancellationToken);
            await this._systemEventsService.RoomCleaningPriorityChanged(this._hotelGroupId, roomId, request.BedId, dateTime, this._userId, request.IsPriority);

            return new ProcessResponseSimple
            {
                IsSuccess = true,
                Message = "Cleaning priority updated.",
            };
        }

        /// Only todays cleaning plan items and cleanings are updated
        private async Task _UpdateCleaningPlanItemsAndCleaningsPriorities(Guid? roomId, Guid? bedId, bool isPriority, DateTime currentHotelLocalTime)
        {
            if (!roomId.HasValue && !bedId.HasValue)
                return;

            var cleaningPlan = await this._databaseContext
                .CleaningPlans
                .Where(cp => cp.Date == currentHotelLocalTime.Date)
                .FirstOrDefaultAsync();

            if (cleaningPlan == null)
                return;


            if (bedId.HasValue)
            {
                var bedItems = await this._databaseContext
                    .CleaningPlanItems
                    .Include(cpi => cpi.Cleaning)
                    .Where(cpi => cpi.CleaningPlanId == cleaningPlan.Id && cpi.RoomBedId == bedId)
                    .ToArrayAsync();

                foreach (var bedItem in bedItems)
                {
                    bedItem.IsPriority = isPriority;

                    if (bedItem.Cleaning != null)
                    {
                        bedItem.Cleaning.IsPriority = isPriority;
                    }
                }
            }
            else if (roomId.HasValue)
            {
                var roomItems = await this._databaseContext
                    .CleaningPlanItems
                    .Include(cpi => cpi.Cleaning)
                    .Where(cpi => cpi.CleaningPlanId == cleaningPlan.Id && cpi.RoomId == roomId && cpi.RoomBedId == null)
                    .ToArrayAsync();

                foreach (var roomItem in roomItems)
                {
                    roomItem.IsPriority = isPriority;

                    if (roomItem.Cleaning != null)
                    {
                        roomItem.Cleaning.IsPriority = isPriority;
                    }
                }
            }
        }
    }
}
