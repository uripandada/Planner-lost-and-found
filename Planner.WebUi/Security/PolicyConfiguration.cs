using Microsoft.Extensions.DependencyInjection;
using Planner.Domain.Values;

namespace Planner.WebUi.Security
{
    public static class PolicyConfiguration
    {
		public static void ConfigurePolicies(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy(ClaimsKeys.ManagementClaimKeys.RoomInsights, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.ManagementClaimKeys.RoomInsights)));
				options.AddPolicy(ClaimsKeys.ManagementClaimKeys.UserInsights, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.ManagementClaimKeys.UserInsights)));
				options.AddPolicy(ClaimsKeys.ManagementClaimKeys.Tasks, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.ManagementClaimKeys.Tasks)));
				options.AddPolicy(ClaimsKeys.ManagementClaimKeys.Reservations, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.ManagementClaimKeys.Reservations)));
				options.AddPolicy(ClaimsKeys.ManagementClaimKeys.ReservationCalendar, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.ManagementClaimKeys.ReservationCalendar)));
				options.AddPolicy(ClaimsKeys.ManagementClaimKeys.LostAndFound, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.ManagementClaimKeys.LostAndFound)));
				options.AddPolicy(ClaimsKeys.ManagementClaimKeys.OnGuard, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.ManagementClaimKeys.OnGuard)));

				options.AddPolicy(ClaimsKeys.SettingsClaimKeys.Rooms, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.SettingsClaimKeys.Rooms)));
				options.AddPolicy(ClaimsKeys.SettingsClaimKeys.Assets, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.SettingsClaimKeys.Assets)));
				options.AddPolicy(ClaimsKeys.SettingsClaimKeys.Users, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.SettingsClaimKeys.Users)));
				options.AddPolicy(ClaimsKeys.SettingsClaimKeys.RoleManagement, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.SettingsClaimKeys.RoleManagement)));
				options.AddPolicy(ClaimsKeys.SettingsClaimKeys.RoomCategories, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.SettingsClaimKeys.RoomCategories)));
				options.AddPolicy(ClaimsKeys.SettingsClaimKeys.HotelSettings, policy => policy.RequireAssertion(context => context.User.HasClaim(x => x.Type == ClaimsKeys.SettingsClaimKeys.HotelSettings)));
			});

		}
	}
}
