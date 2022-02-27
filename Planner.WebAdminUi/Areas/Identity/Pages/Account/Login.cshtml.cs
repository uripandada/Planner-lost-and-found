using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Planner.Common.Extensions;
using Planner.Common.Shared;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        //private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHotelGroupTenantProvider _tenantProvider;

        public LoginModel(SignInManager<User> signInManager, 
            ILogger<LoginModel> logger,
            //IHttpContextAccessor accessor,
            IHotelGroupTenantProvider tenantProvider,
            UserManager<User> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            //httpContextAccessor = accessor;
            this._tenantProvider = tenantProvider;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string HotelGroup { get; set; }

            [Required]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now;
            option.IsEssential = true;
            Response.Cookies.Append("hotel_group_id", Guid.Empty.ToString(), option);

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

			if (Input.HotelGroup.IsNull())
			{
                return Page();
			}

			if (!this._tenantProvider.CheckIfTenantKeyExists(Input.HotelGroup))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
			}
            

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(Input.Email);
                }

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, false);
                    if (result.Succeeded)
                    {
                        var userClaims = await _userManager.GetClaimsAsync(user);
                        var hotelIdClaim = userClaims.Where(x => x.Type == "hotel_group_id").FirstOrDefault();
                        CookieOptions option = new CookieOptions();
                        option.Expires = DateTime.Now.AddDays(60);
                        option.IsEssential = true;
                        Response.Cookies.Append("hotel_group_id", hotelIdClaim.Value, option);

                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
