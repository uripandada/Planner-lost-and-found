using MediatR;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Hotel.Commands.SendTestCleaningNotification
{
	public class SendTestCleaningNotificationCommand: IRequest<ProcessResponse>
	{
	}
	public class SendTestCleaningNotificationCommandHandler : IRequestHandler<SendTestCleaningNotificationCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		public Task<ProcessResponse> Handle(SendTestCleaningNotificationCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
