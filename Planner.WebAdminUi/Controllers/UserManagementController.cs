using Microsoft.AspNetCore.Mvc;
using Planner.Application.Admin.UserManagement.Commands.DeactivateUser;
using Planner.Application.Admin.UserManagement.Commands.InsertUser;
using Planner.Application.Admin.UserManagement.Commands.UpdateUser;
using Planner.Application.Admin.UserManagement.Models;
using Planner.Application.Admin.UserManagement.Queries.GetMasterUserById;
using Planner.Application.Admin.UserManagement.Queries.GetPageOfUsers;
using Planner.Application.Admin.UserManagement.Queries.GetUserDetails;
using Planner.Common.Data;
using System;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Controllers
{
	public class UserManagementController : BaseController
    {
        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse<FullGroupHierarchyData>>> GetFullUserGroupsHierarchy([FromBody] GetFullUserGroupsHierarchyQuery request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpGet]
        //public async Task<ActionResult<ProcessResponse<FullGroupHierarchyData>>> GetGroupsAndSubGroups()
        //{
        //    var result = await this.Mediator.Send(new GetGroupsAndSubGroupsQuery());
        //    return this.Ok(result);
        //}
        

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse<GroupHierarchyData>>> InsertGroup([FromBody] InsertGroupCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse<GroupHierarchyData>>> UpdateGroup([FromBody] UpdateGroupCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse>> DeleteGroup([FromBody] DeleteGroupCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse<SubGroupHierarchyData>>> InsertSubGroup([FromBody] InsertSubGroupCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse<SubGroupHierarchyData>>> UpdateSubGroup([FromBody] UpdateSubGroupCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse>> DeleteSubGroup([FromBody] DeleteSubGroupCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        [HttpGet]
        public async Task<ActionResult<ProcessResponse<MasterUserModel>>> GetUserById(Guid id)
        {
            var result = await this.Mediator.Send(new GetMasterUserByIdQuery { Id = id });
            return this.Ok(result);
        }

        [HttpPost]
        public async Task<UserDetailsData> GetUserDetails(GetUserDetailsQuery query)
        {
            return await this.Mediator.Send(query);
        }
        
        [HttpPost]
        public async Task<PageOf<UserGridData>> GetPageOfUsers(GetPageOfUsersQuery query)
        {
            return await this.Mediator.Send(query);
        }

        [HttpPost]
        public async Task<ProcessResponse<Guid>> InsertUser(InsertUserCommand query)
        {
            return await this.Mediator.Send(query);
        }

        [HttpPost]
        public async Task<ProcessResponse> UpdateUser(UpdateUserCommand query)
        {
            return await this.Mediator.Send(query);
        }

        [HttpPost]
        public async Task<ProcessResponse> DeactivateUser(DeactivateUserCommand query)
        {
            return await this.Mediator.Send(query);
        }

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse<UserModel>>> InsertUser([FromBody] InsertUserCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse<UserModel>>> UpdateUser([FromBody] UpdateUserCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpPost]
        //public async Task<ActionResult<ProcessResponse>> DeleteUser([FromBody] DeleteUserCommand request)
        //{
        //    var result = await this.Mediator.Send(request);
        //    return this.Ok(result);
        //}

        //[HttpPost]
        //public async Task<IEnumerable<UserListItemData>> GetListOfUsers([FromBody] GetListOfUsersQuery request)
        //{
        //    return await this.Mediator.Send(request);
        //}
    }
}
