using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ddPoliglotV6.Areas.Identity.Pages.Account;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using ddPoliglotV6.Infrastructure.Services;
using ddPoliglotV6.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ddPoliglotV6.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly ddPoliglotDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly CommonLocalizationService _localizer;

        public AuthorizationController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            ddPoliglotDbContext context,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            CommonLocalizationService localizer
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
            this._config = config;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        public class UserCredential
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        // 7
        [HttpPost]
        [ActionName("RegisterAnonimusAndGetToken")]
        public async Task<IActionResult> RegisterAnonimusAndGetToken([FromBody] UserCredential userCredential)
        {
            userCredential.UserName = $"Anonim_{Guid.NewGuid().ToString()}";
            userCredential.Password = userCredential.UserName;
            var user = new ApplicationUser { UserName = userCredential.UserName, Email = userCredential.UserName };
            var regResult = await _userManager.CreateAsync(user, userCredential.Password);
            if (!regResult.Succeeded)
            {
                return BadRequest(String.Join(',', regResult.Errors.Select(x=>x.Description).ToList()));
            }

            var result = await _signInManager.PasswordSignInAsync(userCredential.UserName, userCredential.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var tokenData = await (new JwtService(_config, _userManager, _roleManager, _context)).GenerateSecurityToken(userCredential.UserName);
            return Ok(tokenData);
        }

        // 10
        [HttpPost]
        [ActionName("AuthenticateSocialNetwork")]
        public async Task<IActionResult> AuthenticateSocialNetwork([FromBody] UserCredential userCredential)
        {
            userCredential.Password = "115e2989-fcee-4fee-bc30-802a6f663532";
            var user = new ApplicationUser { UserName = userCredential.UserName, Email = userCredential.UserName };

            var existingUser = await _userManager.FindByNameAsync(userCredential.UserName);
            if (existingUser != null)
            {
                // user with this email already exists
                // log with this user
                var token = await (new JwtService(_config, _userManager, _roleManager, _context)).GenerateSecurityToken(userCredential.UserName);
                return Ok(token);
            }
            else {
                // there is no user with this email
                // create new account
                var result = await _userManager.CreateAsync(user, userCredential.Password);
                if (result.Succeeded)
                {
                    var res = await _signInManager.PasswordSignInAsync(userCredential.UserName, userCredential.Password, false, lockoutOnFailure: false);
                    if (!res.Succeeded)
                    {
                        return Unauthorized();
                    }

                    return Ok(await
                        (new JwtService(_config, _userManager, _roleManager, _context))
                        .GenerateSecurityToken(userCredential.UserName)
                        );
                }

                return BadRequest(String.Join(',', result.Errors.Select(x => x.Description).ToList()));
            }
        }

        // 12
        [HttpPost]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("AuthenticateSocialNetworkForAnonim")]
        public async Task<IActionResult> AuthenticateSocialNetworkForAnonim([FromBody] UserCredential userCredential)
        {
            userCredential.Password = "115e2989-fcee-4fee-bc30-802a6f663532";
            var user = new ApplicationUser { UserName = userCredential.UserName, Email = userCredential.UserName };

            var existingUser = await _userManager.FindByNameAsync(userCredential.UserName);
            if (existingUser != null)
            {
                // user with this email already exists
                // log with this user
                var token = await (new JwtService(_config, _userManager, _roleManager, _context)).GenerateSecurityToken(userCredential.UserName);
                return Ok(token);
            }
            else
            {
                // there is no user with this email
                if (User.Identity.IsAuthenticated)
                {
                    if ((User?.Identity?.Name ?? "").StartsWith("Anonim_"))
                    {
                        // create account for this anonimous user.
                        // we need to set name and password for existing user

                        var curUser = await _userManager.FindByNameAsync(User?.Identity?.Name);
                        // change password
                        var token = await _userManager.GeneratePasswordResetTokenAsync(curUser);
                        var pRes = await _userManager.ResetPasswordAsync(curUser, token, userCredential.Password);
                        curUser.UserName = userCredential.UserName;
                        curUser.NormalizedUserName = userCredential.UserName.ToUpper();
                        curUser.Email = userCredential.UserName;
                        curUser.NormalizedEmail = userCredential.UserName.ToUpper();
                        var res = await _userManager.UpdateAsync(curUser);

                        var result = await _signInManager.PasswordSignInAsync(userCredential.UserName, userCredential.Password, false, lockoutOnFailure: false);

                        if (result.Succeeded)
                        {
                            return Ok(await
                                (new JwtService(_config, _userManager, _roleManager, _context))
                                .GenerateSecurityToken(userCredential.UserName)
                                );
                        }

                        return BadRequest(String.Join(',', res.Errors.Select(x => x.Description).ToList()));
                    }
                    else
                    {
                        return BadRequest("User is not anonim and not logout");
                    }
                }
                else
                {
                    // is not loged as anonimous and is not user already with this email
                    // create new account
                    var result = await _userManager.CreateAsync(user, userCredential.Password);
                    if (result.Succeeded)
                    {
                        var res = await _signInManager.PasswordSignInAsync(userCredential.UserName, userCredential.Password, false, lockoutOnFailure: false);

                        if (res.Succeeded)
                        {
                            return Ok(await
                                (new JwtService(_config, _userManager, _roleManager, _context))
                                .GenerateSecurityToken(userCredential.UserName)
                                );
                        }
                    }

                    return BadRequest(String.Join(',', result.Errors.Select(x => x.Description).ToList()));
                }
            }
        }

        // 1, 2, 3
        [HttpPost]
        [ActionName("GetToken")]
        public async Task<IActionResult> GetToken([FromBody] UserCredential userCredential)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredential.UserName, userCredential.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return BadRequest("Login or password is not correct.");
            }

            var tokenData = await (new JwtService(_config, _userManager, _roleManager, _context)).GenerateSecurityToken(userCredential.UserName);
            return Ok(tokenData);
        }

        // 6
        [HttpPost]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("RegisterForAnonim")]
        public async Task<IActionResult> RegisterForAnonim([FromBody] UserCredential userCredential)
        {
            if (string.IsNullOrEmpty(userCredential.UserName) || string.IsNullOrEmpty(userCredential.Password))
            {
                return BadRequest("Empty Data");
            }

            if (User.Identity.IsAuthenticated)
            {
                if ((User?.Identity?.Name ?? "").StartsWith("Anonim_"))
                {
                    // create account for anonimous user.
                    // we need to set name and password for existing user


                    var curUser = await _userManager.FindByNameAsync(User?.Identity?.Name);
                    // change password
                    var token = await _userManager.GeneratePasswordResetTokenAsync(curUser);
                    var pRes = await _userManager.ResetPasswordAsync(curUser, token, userCredential.Password);

                    // curUser = await _userManager.FindByNameAsync(curUser.UserName);

                    curUser.UserName = userCredential.UserName;
                    curUser.NormalizedUserName = userCredential.UserName.ToUpper();
                    curUser.Email = userCredential.UserName;
                    curUser.NormalizedEmail = userCredential.UserName.ToUpper();
                    var res = await _userManager.UpdateAsync(curUser);

                    var result = await _signInManager.PasswordSignInAsync(userCredential.UserName, userCredential.Password, false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        return Ok();
                    }

                    return BadRequest(String.Join(',', res.Errors.Select(x => x.Description).ToList()));
                }
                else
                {
                    return BadRequest("User is not anonim");
                }
            }
            else
            {
                return BadRequest("User is not logout");
            }
        }

        // 4
        [HttpPost]
        [ActionName("register")]
        public async Task<IActionResult> Register([FromBody] UserCredential userCredential)
        {
            if (string.IsNullOrEmpty(userCredential.UserName) || string.IsNullOrEmpty(userCredential.Password))
            {
                return BadRequest("Empty Data");
            }

            var user = new ApplicationUser { UserName = userCredential.UserName, Email = userCredential.UserName };
            var result = await _userManager.CreateAsync(user, userCredential.Password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(String.Join(',', result.Errors.Select(x => x.Description).ToList()));
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [ActionName("GetCurrent")]
        public async Task<IActionResult> GetCurrent()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await this._userManager.FindByNameAsync(User?.Identity?.Name);
                return Ok(new
                {
                    name = User.Identity.Name,
                    id = user.Id,
                    roles = (await this._userManager.GetRolesAsync(user)).ToArray()
                });
            }

            return Unauthorized();
        }

        // get user token from cookie
        [Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
        [HttpGet]
        [ActionName("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity.IsAuthenticated) {
                var user = await this._userManager.FindByNameAsync(User?.Identity?.Name);
                return Ok(new
                {
                    name = User.Identity.Name,
                    id = user.Id,
                    roles = (await this._userManager.GetRolesAsync(user)).ToArray()
                });
            }

            return Unauthorized();
        }
    }
}