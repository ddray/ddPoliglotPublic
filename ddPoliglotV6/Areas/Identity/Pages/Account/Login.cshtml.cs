using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Localization;
using ddPoliglotV6.Infrastructure.Services;

namespace ddPoliglotV6.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly CommonLocalizationService _localizer;
        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            CommonLocalizationService localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _localizer = localizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Display(Name = "xxEmail"), Required(ErrorMessage = "xxEmail Required"), DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [Display(Name = "xxPassword"), Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "xxRemember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;

            returnUrl ??= Url.Content($"~/{requestCulture.Culture.Name}/index");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;

            returnUrl ??= Url.Content($"~/{requestCulture.Culture.Name}/index");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(_localizer.Get("xxUser logged in."));
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(_localizer.Get("xxUser account locked out."));
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer.Get("xxInvalid login attempt."));
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
