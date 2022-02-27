using System;

namespace Planner.Application.UserManagement.Models
{
	public class UserListItemData
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string AvatarImageUrl { get; set; }
        public string FullNameInitials { get; set; }
        public bool IsSubGroupLeader { get; set; }
        public bool IsActive { get; set; }
        public string DefaultAvatarColorHex { get; set; }
    }
}
