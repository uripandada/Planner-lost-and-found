using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Planner.Common.AppSettings;
using Planner.Common.Extensions;
using RestSharp;

namespace Planner.Application.Infrastructure.PusherClient
{

	//"fcm": {
 //       "notification": {
 //       "title": notification,
 //       "icon": "androidlogo"
 //       }
 //   },
 //   "gcm": {
 //       "collapse_key": "roomchecking",
 //       "notification": {
 //       "title": notification,
 //       "icon" : "ic_launcher.png",
 //       "sound": "default"
 //       }
 //   },
 //   "apns": {
 //       "aps": {
 //       "alert": {
 //           "body": notification
 //       },
 //       "sound" : "chime.aiff"
 //       }
 //   },


  //fcm: {
  //  notification: {
  //    title: notification,
  //    icon : "ic_launcher.png",
  //    sound: "default"
  //  }
  //},
  //apns: {
  //  aps: {
  //    alert: {
  //      body: notification
  //    },
  //    sound : "chime.aiff"
  //  }
  //}




	public class PusherBeamsNotification
	{
		public string[] Interests { get; set; }
		public PusherBeamsApns Apns { get; set; }
		public PusherBeamsFcm Fcm { get; set; }
		
		public PusherBeamsNotification(string[] interests, string title, string body)
		{
			this.Interests = interests;
			this.Apns = new PusherBeamsApns
			{
				Aps = new PusherBeamsApnsAps
				{
					Alert = new PusherBeamsApnsApsAlert
					{
						Title = title,
						Body = body,
					}
				}
			};
			this.Fcm = new PusherBeamsFcm
			{
				Notification = new PusherBeamsFcmNotification
				{
					Title = title,
					Body = body,
				}
			};
		}
	}

	public class PusherBeamsApns
	{
		public PusherBeamsApnsAps Aps { get; set; }
	}
	public class PusherBeamsApnsAps
	{
		public PusherBeamsApnsApsAlert Alert { get; set; }
		public string Sound { get; set; } = "chime.aiff";
	}
	public class PusherBeamsApnsApsAlert
	{
		public string Title { get; set; } = "";
		public string Body { get; set; } = "";
	}
	public class PusherBeamsFcm
	{
		public PusherBeamsFcmNotification Notification { get; set; }
	}

	public class PusherBeamsFcmNotification
	{
		public string Title { get; set; } = "";
		public string Body { get; set; } = "";
		public string Icon { get; set; } = "ic_launcher.png";
		public string Sound { get; set; } = "default";
	}
	public class PusherBeamsGcm
	{

	}

	public interface IPusherBeamsClient
	{
		Task SendPushNotification(IEnumerable<Guid> userIds, string title, string message);
	}

	public class PusherBeamsClient: IPusherBeamsClient
	{
		private readonly string AttendantPusherSecretKey;
		private readonly string RequestUrl;

		public PusherBeamsClient(IOptions<PusherBeamsSettings> pusherSettings)
		{
			this.AttendantPusherSecretKey = pusherSettings.Value.AttendantPusherSecretKey;
			this.RequestUrl = pusherSettings.Value.AttendantPusherInterestsUrlTemplate.Replace(pusherSettings.Value.UrlTemplateInstanceIdPlaceholder, pusherSettings.Value.AttendantPusherInstanceId);
		}

		private async Task<HttpResponseMessage> _SendBatchOfNotifications(IEnumerable<Guid> batchOfUserIds, string title, string message)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.AttendantPusherSecretKey}");

			var interests = batchOfUserIds.Select(uid => uid.ToString()).ToArray();
			var data = new PusherBeamsNotification(interests, title, message);

			var json = Newtonsoft.Json.JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
			var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PostAsync(this.RequestUrl, requestContent);

			return response;
		}

		public async Task SendPushNotification(IEnumerable<Guid> userIds, string title, string message)
		{
			foreach(var userIdsPartition in userIds.Partition(100).ToArray())
			{
				var response = await this._SendBatchOfNotifications(userIdsPartition, title, message);

				if (this._ShouldWaitAndResendTheBatch(response))
				{
					await Task.Delay(1000);
					response = await this._SendBatchOfNotifications(userIdsPartition, title, message);
				}

				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var jsonResponse = await response.Content.ReadAsStringAsync();
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
				{
					throw new Exception("Sending push notifications - Bad request");
					// 1. Only application/json is supported.
					// 2. Instance-id param is missing from path.
					// 3. Authorization header is missing.
					// 4. Request body size is too large (max 10KiB).
					// 5. Failed to read body as a JSON object.
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					throw new Exception("Sending push notifications - Incorrect API Key");
					// 1. Incorrect API Key.
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
				{
					throw new Exception("Sending push notifications - Publishing has been blocked due to being over plan limits");
					// 1. Publishing has been blocked due to being over plan limits. More details here
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					throw new Exception("Sending push notifications - 404 - Could not find the instance");
					// 1. Could not find the instance.
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
				{
					throw new Exception("Sending push notifications -  JSON does not our match schema");
					// 1. JSON does not our match schema.
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
				{
					throw new Exception("Sending push notifications - Too many requests being made in quick succession (max 100 RPS)");
					// 1. Too many requests being made in quick succession (max 100 RPS).
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
				{
					throw new Exception("Sending push notifications - Internal server error");
					// 1. Internal server error.
				}

			}
		}

		private bool _ShouldWaitAndResendTheBatch(HttpResponseMessage response)
		{
			return response.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
		}
	}
}
