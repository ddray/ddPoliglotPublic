using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using ddPoliglotV6.Data;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Models;
using Microsoft.Extensions.Configuration;
using ddPoliglotV6.BL.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private TranslationManager _translationManager;

        public TranslateController(ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _translationManager = new TranslationManager(_context, _configuration, _hostingEnvironment);
        }

        [HttpPost]
        [ActionName("translate")]
        public async Task<IActionResult> Translate([FromBody]TranslateArg args)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            args.SourceLanguage = spaAppSetting.LearnLanguage.Code;
            args.TargetLanguage = spaAppSetting.NativeLanguage.Code;

            var result = _translationManager.Translate(args);
            return Ok(new TranslateResult { Message = result});
        }

        [HttpPost]
        [ActionName("translateArray")]
        public async Task<IActionResult> TranslateArray([FromBody]TranslateArrayArg args)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            args.SourceLanguage = spaAppSetting.LearnLanguage.Code;
            args.TargetLanguage = spaAppSetting.NativeLanguage.Code;

            var result = new List<string>();
            result = _translationManager.TranslateArray(args);

            return Ok(result);
        }
    }
}