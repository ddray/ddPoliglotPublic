using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ddPoliglotV6.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;

        public TestingController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
        }

        [HttpGet]
        [ActionName("Test1")]
        public async Task<IActionResult> Test1()
        {
            if (!string.IsNullOrEmpty(User?.Identity?.Name ?? ""))
            {
                var u = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
                //return Ok("test1 response: " + User?.Identity?.Name ?? "sone null");
                return Ok($"test1 response: {u?.Id ?? "sone null"} {u?.UserName ?? "sone null"}");
            }
            else
            {
                return Ok("No current User");
            }
        }

        [Authorize]
        [HttpGet]
        [ActionName("Test2")]
        public async Task<IActionResult> Test2()
        {
            return Ok("test2 response");
        }
    }
}