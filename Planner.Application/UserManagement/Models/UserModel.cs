using Planner.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace Planner.Application.UserManagement.Models
{

	public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConnectionName { get; set; }
        public string RegistrationNumber { get; set; }
        public string Language { get; set; }
        public string OriginalHotel { get; set; }
        public string UserSubGroupId { get; set; }
        public string UserGroupId { get; set; }
        public string PhoneNumber { get; set; }
        public string[] HotelIds { get; set; }
        public string RoleId { get; set; }
        public bool IsSubGroupLeader { get; set; }
        public bool IsActive { get; set; }
        public string DefaultAvatarColorHex { get; set; }
        
        public string AvatarImageUrl { get; set; }

        public static Expression<Func<User, UserModel>> Projection
        {
            get
            {
                return x => new UserModel
                {
                    ConnectionName = x.ConnectionName,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    Id = x.Id.ToString(),
                    Language = x.Language,
                    LastName = x.LastName,
                    OriginalHotel = x.OriginalHotel,
                    RegistrationNumber = x.RegistrationNumber,
                    UserName = x.UserName,
                    UserSubGroupId = x.UserSubGroupId.HasValue ? x.UserSubGroupId.ToString() : null,
                    UserGroupId = x.UserGroupId.HasValue ? x.UserGroupId.ToString() : null,
                    PhoneNumber = x.PhoneNumber,
                    IsSubGroupLeader = x.IsSubGroupLeader,
                    IsActive = x.IsActive,
                    DefaultAvatarColorHex = x.DefaultAvatarColorHex,
                };
            }
        }

        public static UserModel Create(User user)
        {
            return Projection.Compile().Invoke(user);
        }
    }

    public class MasterUserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
