using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Domain.Entities
{
	public class User : IdentityUser<Guid>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ConnectionName { get; set; }
		public string RegistrationNumber { get; set; }
		public string Language { get; set; }
		public string OriginalHotel { get; set; }
		public Guid? UserSubGroupId { get; set; }
		public UserSubGroup UserSubGroup { get; set; }
		public Guid? UserGroupId { get; set; }
		public UserGroup UserGroup { get; set; }

		public bool IsSubGroupLeader { get; set; }
		public bool IsActive { get; set; }

		public string DefaultAvatarColorHex { get; set; }

		public ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
		public ICollection<IdentityUserClaim<Guid>> UserClaims { get; set; }

		public ApplicationUserAvatar Avatar { get; set; }

		public bool IsOnDuty { get; set; }

		public IEnumerable<UserHistoryEvent> UserHistoryEvents { get; set; }
		public IEnumerable<RoomHistoryEvent> RoomHistoryEvents { get; set; }
		public IEnumerable<CleaningHistoryEvent> CleaningHistoryEvents { get; set; }
		public IEnumerable<Cleaning> Cleanings { get; set; }
		public IEnumerable<Cleaning> InspectedCleanings { get; set; }
	}

	public class MasterUser : IdentityUser<Guid>
	{

	}
}
