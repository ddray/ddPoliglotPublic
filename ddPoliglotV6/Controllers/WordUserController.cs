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
    public class WordUserController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private TranslationManager _translationManager;

        public WordUserController(ddPoliglotDbContext context,
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

        [HttpPost]
        [ActionName("updateWordUser")]
        public async Task<IActionResult> UpdateWordUser([FromBody] WordUser wordUser)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            return await UpdateWordUserExec(wordUser, userId);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("update1")]
        public async Task<IActionResult> Update1([FromBody] WordUser wordUser)
        {
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var userId = new Guid(user.Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            wordUser.LanguageID = spaAppSetting.LearnLanguage.LanguageID;
            wordUser.UserID = userId;

            return await UpdateWordUserExec(wordUser, userId);
        }

        private async Task<IActionResult> UpdateWordUserExec(WordUser wordUser, Guid userId)
        {
            var oldWordUser = _context.WordUsers
                .Where(x => x.WordID == wordUser.WordID
                && x.LanguageID == wordUser.LanguageID
                && x.UserID == userId)
                .FirstOrDefault();

            if (oldWordUser == null)
            {
                oldWordUser = new WordUser()
                {
                    Grade = wordUser.Grade,
                    LanguageID = wordUser.LanguageID,
                    UserID = userId,
                    WordID = wordUser.WordID
                };

                _context.Add(oldWordUser);
            }
            else
            {
                oldWordUser.Grade = wordUser.Grade;
                _context.Update(oldWordUser);
            }

            await _context.SaveChangesAsync();
            return Ok(oldWordUser);
        }
    }
}