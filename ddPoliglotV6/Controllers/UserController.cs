using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using NAudio.Wave;
using Microsoft.Extensions.Configuration;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Authorization;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.BL.Managers;
using Microsoft.AspNetCore.Identity;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private TranslationManager _translationManager;

        public UserController(ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _translationManager = new TranslationManager(_context, _configuration, _hostingEnvironment);
            this.UserManager = userManager;
            this.RoleManager = roleManager;
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] ListArg args)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);

            //// prepare sort
            //if (string.IsNullOrEmpty(args.Sort))
            //{
            //    args.Sort = "UserName";
            //}
            //else
            //{
            //    // first letter to upper case
            //    args.Sort = args.Sort.First().ToString().ToUpper() + args.Sort.Substring(1);
            //}

            //if (args.Order == "desc")
            //{
            //    query = query.OrderByDescending(p => EF.Property<object>(p, args.Sort));
            //}
            //else
            //{
            //    query = query.OrderBy(p => EF.Property<object>(p, args.Sort));
            //}


            IQueryable<ApplicationUser> query = UserManager.Users;
            var users = await query.Skip((args.Page) * args.PageSize).Take(args.PageSize).ToListAsync();
            foreach (var user in users)
            {
                user.Roles = (await UserManager.GetRolesAsync(user)).ToList();
            }

            // make query
            var result = new ListResult<ShortUser>()
            {
                Count = await query.CountAsync(),
                Data = users.Select(x => x.MapToShortUser()).ToList()
            };

            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = this.RoleManager.Roles.ToList();

            return Ok(roles.Select(x =>x.Name).ToList());
        }

        [HttpGet]
        [ActionName("GetById")]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            user.Roles = (await UserManager.GetRolesAsync(user)).ToList();
            return Ok(user);
        }

        // set roles to the user
        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save([FromBody] ShortUser args)
        {
            var userInDb = await UserManager.FindByIdAsync(args.id);
            userInDb.Roles = (await UserManager.GetRolesAsync(userInDb)).ToList();

            // remove from roles
            foreach (var role in userInDb.Roles)
            {
                if (!args.roles.Contains(role))
                {
                    await UserManager.RemoveFromRoleAsync(userInDb, role);
                }
            }

            // add to roles
            foreach (string role in args.roles)
            {
                if (!userInDb.Roles.Contains(role))
                {
                    await UserManager.AddToRoleAsync(userInDb, role);
                }
            }

            userInDb = await UserManager.FindByIdAsync(args.id);
            userInDb.Roles = (await UserManager.GetRolesAsync(userInDb)).ToList();

            return Ok(userInDb.MapToShortUser());
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete([FromBody] ShortUser args)
        {
            var userInDb = await UserManager.FindByIdAsync(args.id);
            await UserManager.SetLockoutEndDateAsync(userInDb, new DateTimeOffset(new DateTime(2090, 1, 1)));
            return Ok();
        }
    }
}