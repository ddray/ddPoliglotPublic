using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Linq;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.BL.Models;
using Microsoft.AspNetCore.Authorization;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MixParamTextTempController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public MixParamTextTempController(ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpPost]
        [ActionName("GetFiltered")]
        public async Task<IActionResult> GetFiltered([FromBody] SearchListArg args)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var query = _context.MixParamTextTemps.AsNoTracking()
                .Where(x => x.KeyTemp == args.str1
                && x.LearnLanguageID == spaAppSetting.LearnLanguage.LanguageID
                && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID)
                .Select(x=>x);

            var result = new ListResult<MixParamTextTemp>()
            {
                Count = await query.CountAsync(),
                Data = await query.Skip((args.Page) * args.PageSize).Take(args.PageSize).ToListAsync()
            };

            return Ok(result);
        }


        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save([FromBody] MixParamTextTemp mixParamTextTemp)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            if (mixParamTextTemp.MixParamTextTempID > 0)
            {
                _context.Update(mixParamTextTemp);
            }
            else
            {
                mixParamTextTemp.LearnLanguageID = spaAppSetting.LearnLanguage.LanguageID;
                mixParamTextTemp.NativeLanguageID = spaAppSetting.NativeLanguage.LanguageID;
                _context.Add(mixParamTextTemp);
            }

            _context.SaveChanges();
            return Ok(mixParamTextTemp);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete([FromBody] MixParamTextTemp mixParamTextTemp)
        {
            _context.Remove(mixParamTextTemp);
            _context.SaveChanges();
            return Ok();
        }
    }
}

