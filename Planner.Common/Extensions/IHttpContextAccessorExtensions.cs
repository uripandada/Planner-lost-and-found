using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Common.Extensions
{
	public static class IHttpContextAccessorExtensions
	{
		public static Guid UserId(this IHttpContextAccessor contextAccessor)
		{
			return contextAccessor.HttpContext.UserId();
		}
		public static string RoleName(this IHttpContextAccessor contextAccessor)
		{
			return contextAccessor.HttpContext.RoleName();
		}
		
		public static Guid UserId(this HttpContext context)
		{
			var subClaim = context.User.FindFirst("sub");
			if(subClaim == null)
			{
				subClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
			}

			if (subClaim == null)
				return Guid.Empty;

			return new Guid(subClaim.Value);
		}
		
		public static string RoleName(this HttpContext context)
		{
			var roleClaim = context.User.FindFirst("role");
			if(roleClaim == null)
			{
				roleClaim = context.User.FindFirst(ClaimTypes.Role);
			}

			return roleClaim?.Value;
		}

		public static string TryGetLoginGroupKey(this IHttpContextAccessor contextAccessor)
		{
			if (contextAccessor.HttpContext != null)
			{
				if (contextAccessor.HttpContext.Request.Cookies.ContainsKey("hotel_group_key"))
				{
					return contextAccessor.HttpContext.Request.Cookies["hotel_group_key"];
				}

	//            if(contextAccessor.HttpContext.Request.Path.HasValue && contextAccessor.HttpContext.Request.Path.Value == "/connect/token")
				//{
					if (contextAccessor.HttpContext.Request.Headers.ContainsKey("hotel_group_key"))
					{
						return contextAccessor.HttpContext.Request.Headers["hotel_group_key"];
					}
				//}
			}
			return null;
		}

		public class LoginQueryPartial
		{
			public string HotelGroup { get; set; }
		}

		public static Guid HotelGroupId(this ClaimsPrincipal principal)
		{
			// First check in the authenticated user data - Identity
			var hotelGroupId = principal?.Claims.FirstOrDefault(c => c.Type == "hotel_group_id")?.Value;

			if (hotelGroupId.IsNotNull())
			{ 
				return new Guid(hotelGroupId);
			}
			else
			{
				return Guid.Empty;
			}
		}
		public static Guid Id(this ClaimsPrincipal principal)
		{
			// First check in the authenticated user data - Identity
			var idClaim = principal.FindFirst("sub");
			if (idClaim == null)
			{
				idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
			}
			//var hotelGroupId = principal?.Claims.FirstOrDefault(c => c.Type == "hotel_group_id")?.Value;

			if (idClaim == null)
			{ 
				return Guid.Empty;
			}
			else
			{
				return new Guid(idClaim.Value);
			}
		}

		public static Guid HotelGroupId(this HttpContext context)
		{
			// First check in the authenticated user data - Identity
			var hotelGroupId = context?.User?.FindFirst("hotel_group_id")?.Value;

			if (hotelGroupId.IsNull())
			{
				try
				{
					// Then check the cookies
					hotelGroupId = context?.Request.Cookies["hotel_group_id"];
				}
				finally
				{
				}

				if (hotelGroupId.IsNull())
				{
					return Guid.Empty;
				}
			}

			return new Guid(hotelGroupId);
		}
		public static Guid HotelGroupId(this IHttpContextAccessor contextAccessor)
		{
			return contextAccessor.HttpContext.HotelGroupId();

			//// First check in the authenticated user data - Identity
			//var hotelGroupId = contextAccessor.HttpContext?.User?.FindFirst("hotel_group_id")?.Value;

			//if (hotelGroupId.IsNull())
			//{
			//    try
			//    {
			//        // Then check the cookies
			//        hotelGroupId = contextAccessor.HttpContext?.Request.Cookies["hotel_group_id"];
			//    }
			//    finally
			//    {
			//    }

			//    if (hotelGroupId.IsNull())
			//    {
			//        return Guid.Empty;
			//    }
			//}

			//return new Guid(hotelGroupId);
		}
	}

}
