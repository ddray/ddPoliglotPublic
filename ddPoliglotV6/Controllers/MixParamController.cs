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
    public class MixParamController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public MixParamController(ddPoliglotDbContext context,
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
        [ActionName("SaveMixParam")]
        public async Task<IActionResult> SaveMixParam([FromBody] MixParam newItem)
        {
            // remowe all mixParams for this articlePhrase
            var oldItems = _context.MixParams.Where(x => x.ArticlePhraseKeyGuid == newItem.ArticlePhraseKeyGuid).AsNoTracking().ToList();
            foreach (var item in oldItems)
            {
                _context.Database.ExecuteSqlRaw($"delete from MixItems where MixParamID = {item.MixParamID}");
                _context.Database.ExecuteSqlRaw($"delete from MixParams where MixParamID = {item.MixParamID}");
            }

            // save new
            _context.Add(newItem);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [ActionName("GetByArticlePhraseKeyGuid")]
        public async Task<IActionResult> GetByArticlePhraseKeyGuid([FromBody] ItemArgs args)
        {
            // remowe all mixParams for this articlePhrase
            var item = _context.MixParams.AsNoTracking()
                .Where(x => x.ArticlePhraseKeyGuid == args.str1)
                .Include(y => y.MixItems).AsNoTracking().FirstOrDefault();

            return Ok(item);
        }
    }
}

