using MediatR;
using Microsoft.AspNetCore.Identity;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Commands.DeleteUser
{
	public class DeleteUserCommand : IRequest<DeleteProcessResponse>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteProcessResponse>, IAmWebApplicationHandler
    {
        private readonly UserManager<User> userManager;

        public DeleteUserCommandHandler(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<DeleteProcessResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userManager.FindByIdAsync(request.Id.ToString());

			try
			{
                var deleteResult = await this.userManager.DeleteAsync(user);
            
                if (deleteResult.Succeeded)
                {
                    return new DeleteProcessResponse
                    {
                        IsSuccess = true,
                        HasError = false,
                        HasWarning = false,
                        Message = $"User {user.UserName} deleted"
                    };
                } 
                else
                {
                    return new DeleteProcessResponse
                    {
                        IsSuccess = false,
                        HasError = true,
                        HasWarning = false,
                        Message = "An error occured while deleting user."
                    };
                }
			}
            catch(Exception e)
			{

			}

            user.IsActive = false;
            await this.userManager.UpdateAsync(user);

            return new DeleteProcessResponse
            {
                IsSuccess = false,
                HasWarning = true,
                HasError = false,
                Message = "User deactivated. Unable to delete a user that is referred from another place in the system."
            };
        }
    }
}
