using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Planner.Common.Data;
using System;
using System.Security.Claims;

namespace Planner.WebAdminUi.Controllers
{
	//public static class ControllerExtensions
	//{
	//	public static Guid UserId(this Controller controller)
	//	{
	//		return new Guid(controller.User.FindFirst(ClaimTypes.NameIdentifier).Value);
	//	}

	//	public static string HotelId(this Controller controller)
	//	{
	//		return controller.User.FindFirst("hotel_id").Value;
	//	}
	//}

	[Authorize]
	[ApiController]
	[Route("api/[controller]/[action]")]
	public abstract class BaseController : Controller
	{
		private IMediator _mediator;

		protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());

		protected void _populateErrorModelState(ProcessResponse response, string message)
		{
			foreach (var propertyStateKey in this.ModelState.Keys)
			{
				var propertyState = this.ModelState[propertyStateKey];
				foreach (var error in propertyState.Errors)
				{
					response.AddError(propertyStateKey, error.ErrorMessage);
				}
			}
			response.IsSuccess = false;
			response.HasError = true;
			response.Message = message;
		}
	}
}
