using ddPoliglotV6.BL.Helpers;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ddPoliglotV6.Controllers
{
    [Route("api/[controller]/[action]/{filename}")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;

        public FilesController(
            ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            this.UserManager = userManager;
            this.UserManager = userManager;
            this.RoleManager = roleManager;
        }

        [ActionName("GetPhraseAudio")]
        public async Task<IActionResult> GetPhraseAudio(string fileName)
        {
            var fileFullName = FilesIOHelper.GetPhrasesAudioFolder(_hostingEnvironment.WebRootPath, _configuration) + $"\\{fileName}";
            if (!System.IO.File.Exists(fileFullName))
            {
                return NotFound();
            }

            return PhysicalFile(fileFullName, "audio/mpeg");
        }

        [ActionName("GetArticleAudio")]
        public async Task<IActionResult> GetArticleAudio(string fileName)
        {
            var fileFullName = FilesIOHelper.GetArticlesAudioFolder(_hostingEnvironment.WebRootPath, _configuration) + $"\\{fileName}";
            if (!System.IO.File.Exists(fileFullName))
            {
                return NotFound();
            }

            return PhysicalFile(fileFullName, "audio/mpeg");
        }

        [ActionName("GetArticleVideo")]
        public async Task<IActionResult> GetArticleVideo(string fileName)
        {
            var fileFullName = FilesIOHelper.GetArticlesVideoFolder(_hostingEnvironment.WebRootPath, _configuration) + $"\\{fileName}";
            if (!System.IO.File.Exists(fileFullName))
            {
                return NotFound();
            }

            return PhysicalFile(fileFullName, "video/mp4");
        }

        [ActionName("GetLessonImage")]
        public async Task<IActionResult> GetLessonImage(string fileName)
        {
            var fileFullName = FilesIOHelper.GetLessonImagesFolder(_hostingEnvironment.WebRootPath, _configuration) + $"\\{fileName}";
            if (!System.IO.File.Exists(fileFullName))
            {
                return NotFound();
            }

            return PhysicalFile(fileFullName, "image/jpeg");
        }

        [ActionName("GetLessonAudio")]
        public async Task<IActionResult> GetLessonAudio(string fileName)
        {
            var fileFullName = FilesIOHelper.GetLessonAudioFolder(_hostingEnvironment.WebRootPath, _configuration) + $"\\{fileName}";
            if (!System.IO.File.Exists(fileFullName))
            {
                return NotFound();
            }

            return PhysicalFile(fileFullName, "audio/mpeg");
        }
    }
}