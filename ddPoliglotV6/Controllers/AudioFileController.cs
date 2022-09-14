using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ddPoliglotV6.Controllers
{
    [Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Route("api/[controller]/[action]")]
    public class AudioFileController : Controller
    {
        public async Task<IActionResult> Index(string id)
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/files_mp3",
                id
                );

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            return File(memory, "audio/mpeg3", Path.GetFileName(path));
        }
    }

}
