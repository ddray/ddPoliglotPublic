using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using ddPoliglotV6.BL.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ddPoliglotV6.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private TranslationManager _translationManager;

        public LogController(ddPoliglotDbContext context,
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
        // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Add")]
        public async Task<IActionResult> Add([FromBody] Log log)
        {
            if (User?.Identity?.Name != null)
            {
                var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
                if (user != null)
                {
                    log.UserID = new Guid(user.Id);
                    log.Created = DateTime.Now;
                    _context.Add(log);
                    await _context.SaveChangesAsync();
                }
            }

            log.Created = DateTime.Now;
            _context.Add(log);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}