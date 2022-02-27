using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.Authentication.Queries.Login
{
    public class MasterLoginQuery : IRequest<ProcessResponse<MasterLoginModel>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class MasterLoginModel
    {
        public CookieOptions CookieOptions { get; set; }
    }

    public class MasterLoginQueryHandler : IRequestHandler<MasterLoginQuery, ProcessResponse<MasterLoginModel>>, IAmAdminApplicationHandler
    {
        private readonly UserManager<MasterUser> userManager;
        private readonly SignInManager<MasterUser> signInManager;

        public MasterLoginQueryHandler(UserManager<MasterUser> userManager, SignInManager<MasterUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<ProcessResponse<MasterLoginModel>> Handle(MasterLoginQuery request, CancellationToken cancellationToken)
        {
            var user = await this.userManager.FindByEmailAsync(request.Username);

            if (user == null)
            {
                user = await this.userManager.FindByNameAsync(request.Username);
            }

            if (user != null)
            {
                var result = await this.signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
                if (result.Succeeded)
                {
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(60);
                    option.IsEssential = true;

                    return new ProcessResponse<MasterLoginModel>
                    {
                        Data = new MasterLoginModel
                        {
                            CookieOptions = option,
                        },
                        IsSuccess = true,
                        HasError = false,
                    };
                }
            }

            return new ProcessResponse<MasterLoginModel>
            {
                IsSuccess = false,
                HasError = true,
                Message = "Login failed"
            };
        }
    }
}
