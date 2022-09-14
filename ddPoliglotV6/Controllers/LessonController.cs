using System;
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
using System.Net.Http.Headers;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;
using ddPoliglotV6.BL.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Newtonsoft.Json;
using Google.Apis.YouTube.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using System.Text;
using Google.Apis.Upload;
using System.Diagnostics;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private TranslationManager _translationManager;

        public LessonController(ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _translationManager = new TranslationManager(_context, _configuration, _hostingEnvironment);
            this.UserManager = userManager;
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] SearchListArg args)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);

            // prepare sort
            if (string.IsNullOrEmpty(args.Sort))
            {
                args.Sort = "LessonID";
            }
            else
            {
                // first letter to upper case
                args.Sort = args.Sort.First().ToString().ToUpper() + args.Sort.Substring(1);
            }

            IQueryable<Lesson> query = _context.Lessons.AsNoTracking().Where(x =>
                x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                && x.ParentID == args.parentID
            );

            if (args.Order == "desc")
            {
                query = query.OrderByDescending(p => EF.Property<object>(p, args.Sort));
            }
            else
            {
                query = query.OrderBy(p => EF.Property<object>(p, args.Sort));
            }

            // make query
            var result = new ListResult<Lesson>()
            {
                Count = await query.CountAsync(),
                Data = await query.Skip((args.Page) * args.PageSize).Take(args.PageSize).ToListAsync()
            };

            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var result = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(x => x.LessonID == id);
            if (result.ArticleByParamID > 0)
            {
                result.ArticleByParam = await _context.ArticleByParams.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ArticleByParamID == result.ArticleByParamID);
            }

            return Ok(result);
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save([FromBody] Lesson lesson)
        {
            // verify
            if (_context.Lessons
                .Any(x => x.PageName == lesson.PageName
                    && (lesson.LessonID == 0 || (lesson.LessonID != 0 && x.LessonID != lesson.LessonID))
                )
            )
            {
                throw new Exception("there is already a lesson with this page name");
            }

            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            Lesson oldItem;
            if (lesson.LessonID == 0)
            {
                oldItem = new Lesson();

                // same parent or root
                var lst = _context.Lessons.Where(x =>
                    x.ParentID == 0
                    && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                    && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                    )
                    .Select(x => x.Order).ToList();

                oldItem.Order = lst.Count() > 0 ? (lst.Select(x => x).Cast<int?>().Max() ?? 0) + 1 : 0;

                _context.Add(oldItem);
            }
            else
            {
                oldItem = await _context.Lessons.Where(x => x.LessonID == lesson.LessonID).FirstOrDefaultAsync();
                _context.Update(oldItem);
            }

            oldItem.LanguageID = spaAppSetting.LearnLanguage.LanguageID;
            oldItem.Audio1 = lesson.Audio1;
            oldItem.Audio2 = lesson.Audio2;
            oldItem.Audio3 = lesson.Audio3;
            oldItem.Audio4 = lesson.Audio4;
            oldItem.Audio5 = lesson.Audio5;
            oldItem.Content = lesson.Content;
            oldItem.Description = lesson.Description;
            oldItem.Description1 = lesson.Description1;
            oldItem.Description2 = lesson.Description2;
            oldItem.Description3 = lesson.Description3;
            oldItem.Description4 = lesson.Description4;
            oldItem.Description5 = lesson.Description5;
            oldItem.Name = lesson.Name;
            oldItem.PageName = lesson.PageName;
            oldItem.ParentID = lesson.ParentID;
            oldItem.Video1 = lesson.Video1;
            oldItem.Video2 = lesson.Video2;
            oldItem.Video3 = lesson.Video3;
            oldItem.Video4 = lesson.Video4;
            oldItem.Video5 = lesson.Video5;
            oldItem.Image1 = lesson.Image1;
            oldItem.Image2 = lesson.Image2;
            oldItem.Image3 = lesson.Image3;
            oldItem.Image4 = lesson.Image4;
            oldItem.Image5 = lesson.Image5;
            oldItem.PageMetaTitle = lesson.PageMetaTitle;
            oldItem.PageMetaDescription = lesson.PageMetaDescription;
            oldItem.NativeLanguageID = spaAppSetting.NativeLanguage.LanguageID;
            oldItem.Modified = DateTime.Now;
            oldItem.ArticleByParamID = lesson.ArticleByParamID;
            if (oldItem.LessonID > 0)
            {
                oldItem.Order = lesson.Order;
            }

            _context.SaveChanges();

            return Ok(oldItem);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete([FromBody] Lesson Lesson)
        {
            var oldItem = await _context.Lessons.Where(x => x.LessonID == Lesson.LessonID).FirstOrDefaultAsync();
            if (oldItem.ParentID == 0)
            {
                if (await _context.Lessons.AnyAsync(x => x.ParentID == oldItem.LessonID))
                {
                    throw new Exception("delete children first");
                }
            }

            _context.Remove(oldItem);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [ActionName("GetAudioFileList")]
        public async Task<IActionResult> GetAudioFileList([FromQuery] SearchListArg args)
        {
            var folderPath = args.str1 == "audio"
                ? FilesIOHelper.GetLessonAudioFolder(_hostingEnvironment.WebRootPath, _configuration)
                : args.str1 == "image"
                    ? FilesIOHelper.GetLessonImagesFolder(_hostingEnvironment.WebRootPath, _configuration)
                    : throw new Exception("wrong folder type");

            var list = new List<string>();

            DirectoryInfo d = new DirectoryInfo(folderPath);
            FileInfo[] Files = d.GetFiles("*.*"); //Getting Text files

            foreach (FileInfo file in Files)
            {
                list.Add(file.Name);
            }

            // make query
            var result = new ListResult<string>()
            {
                Count = list.Count(),
                Data = list.Skip((args.Page) * args.PageSize).Take(args.PageSize).ToList()
            };

            return Ok(result);
        }


        [HttpPost, DisableRequestSizeLimit]
        [ActionName("FileUpload")]
        public IActionResult FileUpload([FromQuery] SearchListArg args)
        {
            try
            {
                var file = Request.Form.Files[0];
                var pathToSave = args.str1 == "audio"
                    ? FilesIOHelper.GetLessonAudioFolder(_hostingEnvironment.WebRootPath, _configuration)
                    : args.str1 == "image"
                        ? FilesIOHelper.GetLessonImagesFolder(_hostingEnvironment.WebRootPath, _configuration)
                        : throw new Exception("wrong folder type");

                if (file.Length > 0)
                {
                    if (args.str1 == "image" && args.int1 > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);

                        var ar = fileName.Split('.');
                        var ext = ar[1];
                        var fileNameTmp = $"Thumb__{ar[0]}.{ar[1]}";
                        var fullPath_tmp = Path.Combine(pathToSave, fileNameTmp);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // image with resizing
                        using (var image = Image.FromFile(fullPath))
                        {
                            var ratio = (double)args.int1 / image.Width;
                            var newWidth = (int)(image.Width * ratio);
                            var newHeight = (int)(image.Height * ratio);
                            var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

                            // Draws the image in the specified size with quality mode set to HighQuality
                            using (Graphics graphics = Graphics.FromImage(newImage))
                            {
                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                            }

                            ImageCodecInfo imageCodecInfo = null;
                            var imageFormat = ImageFormat.Jpeg;
                            // Get an ImageCodecInfo object that represents the JPEG codec.
                            if (ext == "jpg" || ext == "jpeg")
                            {
                                imageCodecInfo = this.GetEncoderInfo(ImageFormat.Jpeg);
                                imageFormat = ImageFormat.Jpeg;
                            }
                            else if (ext == "png")
                            {
                                imageCodecInfo = this.GetEncoderInfo(ImageFormat.Png);
                                imageFormat = ImageFormat.Png;
                            }
                            else if (ext == "gif")
                            {
                                imageCodecInfo = this.GetEncoderInfo(ImageFormat.Gif);
                                imageFormat = ImageFormat.Gif;
                            }

                            //// Create an Encoder object for the Quality parameter.
                            //Encoder encoder = Encoder.Quality;

                            //// Create an EncoderParameters object. 
                            //EncoderParameters encoderParameters = new EncoderParameters(1);

                            //// Save the image as a JPEG file with quality level.
                            //EncoderParameter encoderParameter = new EncoderParameter(encoder, 75);
                            //encoderParameters.Param[0] = encoderParameter;
                            //newImage.Save(fullPath_tmp, imageCodecInfo, encoderParameters);
                            newImage.Save(fullPath_tmp, imageFormat);
                        }
                    }
                    else
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

            return Ok();
        }

        [HttpPost]
        [ActionName("GenerateFromArticles")]
        public async Task<IActionResult> GenerateFromArticles([FromBody] ArtParamsGenerationArg args)
        {
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid(user.Id);

            var articles = _context.Articles
                .Where(x => x.Name.StartsWith(args.baseName))
                .OrderBy(x => x.Name)
                .ToList();

            var artDrs = new List<articleGr>();
            foreach (var article in articles)
            {
                // en_v1_lesson-0010_2
                var ar = article.Name.Split(args.baseName, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Replace("-", "").Split("_", StringSplitOptions.RemoveEmptyEntries);
                artDrs.Add(new articleGr()
                {
                    Article = article,
                    Num = Convert.ToInt32(ar[0]),
                    Part = Convert.ToInt32(ar[1])
                });
            }

            var gr = from ag in artDrs
                     group ag by ag.Num into agGroup
                     where 
                        agGroup.Key >= args.startLessonNum 
                        && agGroup.Key <= args.endLessonNum
                     orderby agGroup.Key
                     select agGroup;

            // validate
            foreach (var items in gr)
            {
                var parts = items.OrderBy(x => x.Part).ToList();

                var lessonNum = parts[0].Num;
                var numString = lessonNum.ToString("#0000");
                var articleByParam = _context.ArticleByParams.FirstOrDefault(x =>
                    x.LearnLanguageID == spaAppSetting.LearnLanguage.LanguageID
                    && x.Name == $"{args.baseName}-{numString}"
                    && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                    && x.UserID == userId
                );

                if (_context.Lessons.Any(x=>
                        x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                        && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                        && x.Order == lessonNum
                        && x.ArticleByParamID == articleByParam.ArticleByParamID)
                    )
                {
                    throw new Exception($"such lesson with num {lessonNum} already exists");
                }

                var withoutAudioAndVideo = items.Where(x =>
                    x.Article.TextSpeechFileName.Length < 1
                    || x.Article.VideoFileName.Length < 1)
                    .ToList();

                if (withoutAudioAndVideo.Count() > 0)
                {
                    throw new Exception($"such articles without Video or Audio generated : {string.Join(',', withoutAudioAndVideo.Select(x=>x.Article.Name))}");
                }
            }

            // create lessons
            foreach (var items in gr)
            {
                var parts = items.OrderBy(x => x.Part).ToList();

                var lessonNum = parts[0].Num;
                var numString = lessonNum.ToString("#0000");
                var articleByParam = _context.ArticleByParams.FirstOrDefault(x =>
                    x.LearnLanguageID == spaAppSetting.LearnLanguage.LanguageID
                    && x.Name == $"{args.baseName}-{numString}" 
                    && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                    && x.UserID == userId
                );

                var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(articleByParam.DataJson);
                var wordsStr = "";
                foreach (var wordSelected in articleByParamData.SelectedWords)
                {
                    if (wordsStr == "")
                    {
                        wordsStr = wordSelected.Word.Text;
                    }
                    else
                    {
                        wordsStr += $", {wordSelected.Word.Text}";
                    }
                }

                var startRate = articleByParamData.SelectedWords.OrderBy(x => x.Word.Rate).First().Word.Rate;
                var endRate = articleByParamData.SelectedWords.OrderBy(x => x.Word.Rate).Last().Word.Rate;
                var lesson = new Lesson();
                lesson.LanguageID = spaAppSetting.LearnLanguage.LanguageID;
                lesson.NativeLanguageID = spaAppSetting.NativeLanguage.LanguageID;
                lesson.PageName = $"urok-{lessonNum}";
                lesson.Name = $"Тренажер Английского. Урок {lessonNum}. Учим слова с {startRate} по {endRate} ({wordsStr}) .";
                lesson.PageMetaDescription = $"Тренажер Английского. Урок {lessonNum}. Учим слова с {startRate} по {endRate} ({wordsStr}) .";
                lesson.Title = $"Урок {lessonNum} английского языка. Тренажер по изучению слов ({wordsStr})";
                lesson.PageMetaTitle = $"Урок {lessonNum} английского языка. Тренажер по изучению слов ({wordsStr})";
                lesson.Description = $"На этом уроке английского языка мы изучим очередные пять слов с {startRate} по {endRate}";
                lesson.Content = "";
                lesson.Order = lessonNum;
                lesson.ArticleByParamID = articleByParam.ArticleByParamID;

                var imageNumText = lessonNum.ToString();
                var imageName = $"{args.baseName}-{numString}_1.jpg";
                var imageNameThump = $"Thumb__{args.baseName}-{numString}_1.jpg";
                ImageHelper.GenerateLessonImages(imageName, imageNameThump, imageNumText, _hostingEnvironment.WebRootPath, _configuration);

                lesson.Video1 = "";
                lesson.Audio1 = parts[0].Article.TextSpeechFileName;
                lesson.Image1 = imageNameThump;
                lesson.Description1 = "Это версия урока, которая включает в себя все четыре упражнения в одном файле.";

                if (parts.Count > 1)
                {
                    imageNumText = $"{lessonNum.ToString()}.1";
                    imageName = $"{args.baseName}-{numString}_2.jpg";
                    imageNameThump = $"Thumb__{args.baseName}-{numString}_2.jpg";
                    ImageHelper.GenerateLessonImages(imageName, imageNameThump, imageNumText, _hostingEnvironment.WebRootPath, _configuration);

                    lesson.Video2 = "";
                    lesson.Audio2 = parts[1].Article.TextSpeechFileName;
                    lesson.Image2 = imageNameThump;
                    lesson.Description2 = "Первая часть урока в отдельном виде. Знакомимся с новым материалом, тренеруемся понимать и произносить.";
                }

                if (parts.Count > 2)
                {
                    imageNumText = $"{lessonNum.ToString()}.2";
                    imageName = $"{args.baseName}-{numString}_3.jpg";
                    imageNameThump = $"Thumb__{args.baseName}-{numString}_3.jpg";
                    ImageHelper.GenerateLessonImages(imageName, imageNameThump, imageNumText, _hostingEnvironment.WebRootPath, _configuration);

                    lesson.Video3 = "";
                    lesson.Audio3 = parts[2].Article.TextSpeechFileName;
                    lesson.Image3 = imageNameThump;
                    lesson.Description3 = "Вторая часть. Второе и третье упражнения урока в отдельных видео и аудио. Заучиваем новые слова, тренеруемся понимать и произносить.";
                }

                if (parts.Count > 3)
                {
                    imageNumText = $"{lessonNum.ToString()}.3";
                    imageName = $"{args.baseName}-{numString}_4.jpg";
                    imageNameThump = $"Thumb__{args.baseName}-{numString}_4.jpg";
                    ImageHelper.GenerateLessonImages(imageName, imageNameThump, imageNumText, _hostingEnvironment.WebRootPath, _configuration);

                    lesson.Video4 = "";
                    lesson.Audio4 = parts[3].Article.TextSpeechFileName;
                    lesson.Image4 = imageNameThump;
                    lesson.Description4 = "Третья часть. Четвертое упражнения урока в отдельном видео и аудио. Заучиваем новые фразы.";
                }

                _context.Lessons.Add(lesson);
                _context.SaveChanges();
            }
               
            return Ok();
        }

        [HttpPost]
        [ActionName("uploadVideosToYoutube")]
        public async Task<IActionResult> UploadVideosToYoutube([FromBody] ArtParamsGenerationArg args)
        {
            // correct video title and descriptions for existing 
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid(user.Id);

            var articles = _context.Articles
                .Where(x => x.Name.StartsWith(args.baseName))
                .OrderBy(x => x.Name)
                .ToList();

            var artDrs = new List<articleGr>();
            foreach (var article in articles)
            {
                // en_v1_lesson-0010_2
                var ar = article.Name.Split(args.baseName, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Replace("-", "").Split("_", StringSplitOptions.RemoveEmptyEntries);
                artDrs.Add(new articleGr()
                {
                    Article = article,
                    Num = Convert.ToInt32(ar[0]),
                    Part = Convert.ToInt32(ar[1])
                });
            }

            var gr = from ag in artDrs
                     group ag by ag.Num into agGroup
                     where
                        agGroup.Key >= args.startLessonNum
                        && agGroup.Key <= args.endLessonNum
                     orderby agGroup.Key
                     select agGroup;

            // validate
            foreach (var items in gr)
            {
                var parts = items.OrderBy(x => x.Part).ToList();

                var lessonNum = parts[0].Num;
                var numString = lessonNum.ToString("#0000");
                var articleByParam = _context.ArticleByParams.FirstOrDefault(x =>
                    x.LearnLanguageID == spaAppSetting.LearnLanguage.LanguageID
                    && x.Name == $"{args.baseName}-{numString}"
                    && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                    && x.UserID == userId
                );

                if (!_context.Lessons.Any(x =>
                        x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                        && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                        && x.Order == lessonNum
                        && x.ArticleByParamID == articleByParam.ArticleByParamID)
                    )
                {
                    throw new Exception($"such lesson with num {lessonNum} is not exists");
                }

                var withoutAudioAndVideo = items.Where(x =>
                    x.Article.TextSpeechFileName.Length < 1
                    || x.Article.VideoFileName.Length < 1)
                    .ToList();

                if (withoutAudioAndVideo.Count() > 0)
                {
                    throw new Exception($"such articles without Video or Audio generated : {string.Join(',', withoutAudioAndVideo.Select(x => x.Article.Name))}");
                }
            }

            // get list of youtube videos already presets
            var existsYouTubeIsd = new List<string>();
            List<Lesson> lessons;   
            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                lessons = ent.Lessons.Where(x => 
                    x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                    && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                    ).ToList();
            }

            foreach (var lesson in lessons)
            {
                if (!string.IsNullOrWhiteSpace(lesson.Video1))
                {
                    existsYouTubeIsd.Add(lesson.Video1);
                }
                if (!string.IsNullOrWhiteSpace(lesson.Video2))
                {
                    existsYouTubeIsd.Add(lesson.Video2);
                }
                if (!string.IsNullOrWhiteSpace(lesson.Video3))
                {
                    existsYouTubeIsd.Add(lesson.Video3);
                }
                if (!string.IsNullOrWhiteSpace(lesson.Video4))
                {
                    existsYouTubeIsd.Add(lesson.Video4);
                }
            }

            var youTubeService = await GetYouTubeService();

            var videosYouTube = new List<Video>();
            var videoIDs = "";
            var nextPageToken = "";
            var cnt = 0;
            while (nextPageToken != null)
            {
                var playlistItemsListRequest = youTubeService.PlaylistItems.List("snippet");
                playlistItemsListRequest.PlaylistId = "PLfD0U4aAv0_KqCnKFHf2W66FwFfQvR5cg";
                playlistItemsListRequest.MaxResults = 40;
                playlistItemsListRequest.PageToken = nextPageToken;

                // Retrieve the list of videos uploaded to the authenticated user's channel.
                var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

                foreach (var playlistItem in playlistItemsListResponse.Items)
                {
                    if (!existsYouTubeIsd.Contains(playlistItem.Snippet.ResourceId.VideoId))
                    {
                        if (videoIDs == "")
                        {
                            videoIDs = playlistItem.Snippet.ResourceId.VideoId;
                        }
                        else
                        {
                            videoIDs += "," + playlistItem.Snippet.ResourceId.VideoId;
                        }
                    }

                    if (cnt++ > 20)
                    {
                        var vReq1 = youTubeService.Videos.List("snippet, fileDetails");
                        vReq1.Id = videoIDs;
                        var videoResp1 = vReq1.Execute();

                        videosYouTube.AddRange(videoResp1.Items);
                        videoIDs = "";
                        cnt = 0;
                    }
                }

                nextPageToken = playlistItemsListResponse.NextPageToken;
            }

            if (!string.IsNullOrEmpty(videoIDs))
            {
                var vReq = youTubeService.Videos.List("snippet, fileDetails");
                vReq.Id = videoIDs;
                var videoResp = vReq.Execute();

                videosYouTube.AddRange(videoResp.Items);
            }

            foreach (var vid in videosYouTube)
            {
                // link with article
                var articleGr = artDrs.FirstOrDefault(x => x.Article.VideoFileName == vid.FileDetails.FileName);
                if (articleGr == null)
                {
                    var les = _context.Lessons.AsNoTracking().FirstOrDefault(
                        x=>x.Video1 == vid.Id
                        || x.Video2 == vid.Id
                        || x.Video3 == vid.Id
                        || x.Video4 == vid.Id
                        );

                    if (les != null)
                    {
                        var num = les.Order;
                        var partNum = les.Video1 == vid.Id
                            ? 1
                            : les.Video2 == vid.Id
                                ? 2
                                : les.Video3 == vid.Id
                                ? 3
                                : les.Video4 == vid.Id
                                    ? 4 : 0;
                        articleGr = artDrs.FirstOrDefault(x => x.Num == num && x.Part == partNum);
                    }
                }

                if (articleGr != null)
                {
                    articleGr.Video = vid;
                }

                //// by file name
                //if (fileName.StartsWith(args.baseName))
                //{
                //    var ar = fileName.Split(args.baseName, StringSplitOptions.RemoveEmptyEntries);
                //    var ar1 = ar[0].Split("_", StringSplitOptions.RemoveEmptyEntries);
                //    var lessonNum = Convert.ToInt32(ar1[0]);
                //    var partNum = Convert.ToInt32(ar1[1]);
                //    var articleGr = artDrs.FirstOrDefault(x=>x.Num == lessonNum && x.Part == partNum);
                //    articleGr.Video = vid;
                //}
            }



            //var channelsListRequest = youTubeService.Channels.List("contentDetails");
            //channelsListRequest.Mine = true;

            //// Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
            //var channelsListResponse = await channelsListRequest.ExecuteAsync();

            //foreach (var channel in channelsListResponse.Items)
            //{
            //    // From the API response, extract the playlist ID that identifies the list
            //    // of videos uploaded to the authenticated user's channel.
            //    var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;

            //    Console.WriteLine("Videos in list {0}", uploadsListId);

            //    var nextPageToken = "";
            //    while (nextPageToken != null)
            //    {
            //        var playlistItemsListRequest = youTubeService.PlaylistItems.List("snippet");
            //        playlistItemsListRequest.PlaylistId = uploadsListId;
            //        playlistItemsListRequest.MaxResults = 50;
            //        playlistItemsListRequest.PageToken = nextPageToken;

            //        // Retrieve the list of videos uploaded to the authenticated user's channel.
            //        var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

            //        foreach (var playlistItem in playlistItemsListResponse.Items)
            //        {
            //            // Print information about each video.
            //            Console.WriteLine("{0} ({1})", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId);
            //            // get video - to last when other will be awailable
            //            var vReq = youTubeService.Videos.List("contentDetails, snippet, fileDetails");
            //            vReq.Id = playlistItem.Snippet.ResourceId.VideoId;
            //            var videoResp = vReq.Execute();
            //        }

            //        nextPageToken = playlistItemsListResponse.NextPageToken;
            //    }
            //}


            // validation 2
            foreach (var items in gr)
            {
                var parts = items.OrderBy(x => x.Part).ToList();
                foreach (var part in parts)
                {
                    if (part.Part == 1) // for 1 only
                    {
                        if (part.Video == null)
                        {
                            throw new Exception($"such articles without Video: {part.Article.Name}");
                        }
                    }
                }
            }

            foreach (var items in gr)
            {
                var parts = items.OrderBy(x => x.Part).ToList();

                if (parts.Count != 4)
                {
                    // for 4 parts only
                    continue;
                }

                var lessonNum = parts[0].Num;
                var numString = lessonNum.ToString("#0000");
                var articleByParam = _context.ArticleByParams.FirstOrDefault(x =>
                    x.LearnLanguageID == spaAppSetting.LearnLanguage.LanguageID
                    && x.Name == $"{args.baseName}-{numString}"
                    && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                    && x.UserID == userId
                );

                var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(articleByParam.DataJson);
                var currentWordsSB = new StringBuilder();
                var shortCurrentPhrasesSB = new StringBuilder();
                var phCnt = 0;
                foreach (var wordSelected in articleByParamData.SelectedWords)
                {
                    currentWordsSB.AppendLine("        " + wordSelected.Word.Text);

                    if (phCnt++ < 4)
                    {
                        shortCurrentPhrasesSB.AppendLine("        " + wordSelected.Word.WordPhraseSelected[0].Text);
                    }
                }

                shortCurrentPhrasesSB.AppendLine("        " + "...");

                var repeatWordsSB = new StringBuilder();
                phCnt = 0;
                foreach (var wordToRepeat in articleByParamData.WordsToRepeat)
                {
                    if (phCnt++ < 6)
                    {
                        repeatWordsSB.AppendLine("        " + wordToRepeat.Text);
                    }
                }

                repeatWordsSB.AppendLine("        " + "...");


                var shortRepeatPhrasesSB = new StringBuilder();
                phCnt = 0;
                foreach (var wordPhraseToRepeat in articleByParamData.WordPhrasesToRepeat)
                {
                    if (phCnt++ < 6)
                    {
                        shortRepeatPhrasesSB.AppendLine("        " + wordPhraseToRepeat.Text);
                    }
                }

                shortRepeatPhrasesSB.AppendLine("        " + "...");


                var lesson = _context.Lessons.FirstOrDefault(x =>
                        x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                        && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                        && x.Order == lessonNum
                        && x.ArticleByParamID == articleByParam.ArticleByParamID);

                lesson.Video1 = parts[0].Video.Id;

                // for 1 part only
                //lesson.Video2 = parts[1].Video.Id;
                //lesson.Video3 = parts[2].Video.Id;
                //lesson.Video4 = parts[3].Video.Id;

                _context.Update(lesson);
                _context.SaveChanges();

                var firsWordNum = (((lessonNum - 1) * 5) + 1);
                var lastWordNum = firsWordNum + 4;
                var youTubeVideos_Part1 = $"https://www.youtube.com/watch?v={lesson.Video1}";
                var youTubeVideos_Part2 = $"https://www.youtube.com/watch?v={lesson.Video2}";
                var youTubeVideos_Part3 = $"https://www.youtube.com/watch?v={lesson.Video3}";
                var youTubeVideos_Part4 = $"https://www.youtube.com/watch?v={lesson.Video4}";

                var title1 = FilesIOHelper.GetRndLine(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeName_1.txt");
                title1 = title1
                    .Replace("##lessonNum##", lessonNum.ToString())
                    .Replace("##firsWordNum##", firsWordNum.ToString())
                    .Replace("##lastWordNum##", lastWordNum.ToString())
                    ;

                var title2 = FilesIOHelper.GetRndLine(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeName_2.txt");
                title2 = title2
                    .Replace("##lessonNum##", lessonNum.ToString())
                    .Replace("##firsWordNum##", firsWordNum.ToString())
                    .Replace("##lastWordNum##", lastWordNum.ToString())
                    ;

                var title3 = FilesIOHelper.GetRndLine(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeName_3.txt");
                title3 = title3
                    .Replace("##lessonNum##", lessonNum.ToString())
                    .Replace("##firsWordNum##", firsWordNum.ToString())
                    .Replace("##lastWordNum##", lastWordNum.ToString())
                    ;

                var title4 = FilesIOHelper.GetRndLine(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeName_4.txt");
                title4 = title4
                    .Replace("##lessonNum##", lessonNum.ToString())
                    .Replace("##firsWordNum##", firsWordNum.ToString())
                    .Replace("##lastWordNum##", lastWordNum.ToString())
                    ;
                // for 1 part only
                var description1 = FilesIOHelper.GetTextFromFile(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeDescription_1_1.txt");
                description1 = description1
                    .Replace("##lessonNum##", lessonNum.ToString())
                    .Replace("##firsWordNum##", firsWordNum.ToString())
                    .Replace("##lastWordNum##", lastWordNum.ToString())
                    .Replace("##YouTubeVideos_Part1##", youTubeVideos_Part1)
                    .Replace("##YouTubeVideos_Part2##", youTubeVideos_Part2)
                    .Replace("##YouTubeVideos_Part3##", youTubeVideos_Part3)
                    .Replace("##YouTubeVideos_Part4##", youTubeVideos_Part4)
                    .Replace("##CurrentWords##", currentWordsSB.ToString())
                    .Replace("##ShortCurrentPhrases##", shortCurrentPhrasesSB.ToString())
                    .Replace("##RepeatWords##", repeatWordsSB.ToString())
                    .Replace("##ShortRepeatPhrases##", shortRepeatPhrasesSB.ToString())
                    ;

                var description2 = FilesIOHelper.GetTextFromFile(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeDescription_2.txt");
                description2 = description2
                    .Replace("##lessonNum##", lessonNum.ToString())
                    .Replace("##firsWordNum##", firsWordNum.ToString())
                    .Replace("##lastWordNum##", lastWordNum.ToString())
                    .Replace("##YouTubeVideos_Part1##", youTubeVideos_Part1)
                    .Replace("##YouTubeVideos_Part2##", youTubeVideos_Part2)
                    .Replace("##YouTubeVideos_Part3##", youTubeVideos_Part3)
                    .Replace("##YouTubeVideos_Part4##", youTubeVideos_Part4)
                    .Replace("##CurrentWords##", currentWordsSB.ToString())
                    .Replace("##ShortCurrentPhrases##", shortCurrentPhrasesSB.ToString())
                    .Replace("##RepeatWords##", repeatWordsSB.ToString())
                    .Replace("##ShortRepeatPhrases##", shortRepeatPhrasesSB.ToString())
                    ;

                var description3 = FilesIOHelper.GetTextFromFile(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeDescription_3.txt");
                description3 = description3
                    .Replace("##lessonNum##", lessonNum.ToString())
                    .Replace("##firsWordNum##", firsWordNum.ToString())
                    .Replace("##lastWordNum##", lastWordNum.ToString())
                    .Replace("##YouTubeVideos_Part1##", youTubeVideos_Part1)
                    .Replace("##YouTubeVideos_Part2##", youTubeVideos_Part2)
                    .Replace("##YouTubeVideos_Part3##", youTubeVideos_Part3)
                    .Replace("##YouTubeVideos_Part4##", youTubeVideos_Part4)
                    .Replace("##CurrentWords##", currentWordsSB.ToString())
                    .Replace("##ShortCurrentPhrases##", shortCurrentPhrasesSB.ToString())
                    .Replace("##RepeatWords##", repeatWordsSB.ToString())
                    .Replace("##ShortRepeatPhrases##", shortRepeatPhrasesSB.ToString())
                    ;

                var description4 = FilesIOHelper.GetTextFromFile(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeDescription_4.txt");
                description4 = description4
                    .Replace("##lessonNum##", lessonNum.ToString())
                    .Replace("##firsWordNum##", firsWordNum.ToString())
                    .Replace("##lastWordNum##", lastWordNum.ToString())
                    .Replace("##YouTubeVideos_Part1##", youTubeVideos_Part1)
                    .Replace("##YouTubeVideos_Part2##", youTubeVideos_Part2)
                    .Replace("##YouTubeVideos_Part3##", youTubeVideos_Part3)
                    .Replace("##YouTubeVideos_Part4##", youTubeVideos_Part4)
                    .Replace("##CurrentWords##", currentWordsSB.ToString())
                    .Replace("##ShortCurrentPhrases##", shortCurrentPhrasesSB.ToString())
                    .Replace("##RepeatWords##", repeatWordsSB.ToString())
                    .Replace("##ShortRepeatPhrases##", shortRepeatPhrasesSB.ToString())
                    ;

                var tags = new string[] { "Уроки английского языка", "Тренажер английского", "уровень английского языка A1" };
                var categoryId = "27";

                // part1
                var video = parts[0].Video;
                if (
                    video.Snippet.Description != description1
                    )
                {
                    video.ContentDetails = null;
                    video.FileDetails = null;
                    video.Snippet.Title = title1;
                    video.Snippet.Description = description1;
                    video.Snippet.Tags = tags;
                    video.Snippet.CategoryId = categoryId;
                    await youTubeService.Videos.Update(video, "snippet").ExecuteAsync();
                }

                //////// for 1 part only
                //////// part2
                //////video = parts[1].Video;
                //////if (
                //////    video.Snippet.Description != description2
                //////    )
                //////{
                //////    video.Snippet.Title = title2;
                //////    video.Snippet.Description = description2;
                //////    video.Snippet.Tags = tags;
                //////    video.Snippet.CategoryId = categoryId;
                //////    video.ContentDetails = null;
                //////    video.FileDetails = null;
                //////    await youTubeService.Videos.Update(video, "snippet").ExecuteAsync();
                //////}

                //////// part3
                //////video = parts[2].Video;
                //////if (
                //////    video.Snippet.Description != description3
                //////    )
                //////{
                //////    video.Snippet.Title = title3;
                //////    video.Snippet.Description = description3;
                //////    video.Snippet.Tags = tags;
                //////    video.ContentDetails = null;
                //////    video.FileDetails = null;
                //////    video.Snippet.CategoryId = categoryId;
                //////    await youTubeService.Videos.Update(video, "snippet").ExecuteAsync();
                //////}

                //////// part4
                //////video = parts[3].Video;
                //////if (
                //////    video.Snippet.Description != description4
                //////    )
                //////{
                //////    video.Snippet.Title = title4;
                //////    video.Snippet.Description = description4;
                //////    video.Snippet.Tags = tags;
                //////    video.Snippet.CategoryId = categoryId;
                //////    video.ContentDetails = null;
                //////    video.FileDetails = null;
                //////    await youTubeService.Videos.Update(video, "snippet").ExecuteAsync();
                //////}


                //var videoNew2 = new Video();
                //videoNew2.Snippet = new VideoSnippet();
                //videoNew2.Snippet.Title = title2;
                //videoNew2.Snippet.Description = description2;
                //videoNew2.Snippet.Tags = new string[] { "Уроки английского языка", "Тренажер английского", "уровень английского языка A1" };
                //videoNew2.Snippet.CategoryId = "27"; // education
                //videoNew2.Status = new VideoStatus();
                //videoNew2.Status.PrivacyStatus = "public"; // or "private" or "public"
                //var videoFileName2 = parts[1].Article.VideoFileName;
                //var filePath2 = @"D:\000_Work_7\ddPoliglot\files-repo-important\articles-video\" + videoFileName2;

                //this.lastUploadedVideo = null;
                //using (var fileStream = new FileStream(filePath2, FileMode.Open))
                //{
                //    var videosInsertRequest = youTubeService.Videos.Insert(videoNew2, "snippet,status", fileStream, "video/*");
                //    videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                //    videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                //    var result = await videosInsertRequest.UploadAsync();
                //}

                //if (this.lastUploadedVideo == null)
                //{
                //    throw new Exception("upload video error");
                //}

                //lesson.Video2 = this.lastUploadedVideo.Id;

                //// part3


                //var videoNew3 = new Video();
                //videoNew3.Snippet = new VideoSnippet();
                //videoNew3.Snippet.Title = title3;
                //videoNew3.Snippet.Description = description3;
                //videoNew3.Snippet.Tags = new string[] { "Уроки английского языка", "Тренажер английского", "уровень английского языка A1" };
                //videoNew3.Snippet.CategoryId = "27"; // education
                //videoNew3.Status = new VideoStatus();
                //videoNew3.Status.PrivacyStatus = "public"; // or "private" or "public"
                //var videoFileName3 = parts[2].Article.VideoFileName;
                //var filePath3 = @"D:\000_Work_7\ddPoliglot\files-repo-important\articles-video\" + videoFileName3;

                //this.lastUploadedVideo = null;
                //using (var fileStream = new FileStream(filePath3, FileMode.Open))
                //{
                //    var videosInsertRequest = youTubeService.Videos.Insert(videoNew3, "snippet,status", fileStream, "video/*");
                //    videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                //    videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                //    var result = await videosInsertRequest.UploadAsync();
                //}

                //if (this.lastUploadedVideo == null)
                //{
                //    throw new Exception("upload video error");
                //}

                //lesson.Video3 = this.lastUploadedVideo.Id;

                //// part4


                //var videoNew4 = new Video();
                //videoNew4.Snippet = new VideoSnippet();
                //videoNew4.Snippet.Title = title4;
                //videoNew4.Snippet.Description = description4;
                //videoNew4.Snippet.Tags = new string[] { "Уроки английского языка", "Тренажер английского", "уровень английского языка A1" };
                //videoNew4.Snippet.CategoryId = "27"; // education
                //videoNew4.Status = new VideoStatus();
                //videoNew4.Status.PrivacyStatus = "public"; // or "private" or "public"
                //var videoFileName4 = parts[3].Article.VideoFileName;
                //var filePath4 = @"D:\000_Work_7\ddPoliglot\files-repo-important\articles-video\" + videoFileName4;

                //this.lastUploadedVideo = null;
                //using (var fileStream = new FileStream(filePath4, FileMode.Open))
                //{
                //    var videosInsertRequest = youTubeService.Videos.Insert(videoNew4, "snippet,status", fileStream, "video/*");
                //    videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                //    videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                //    var result = await videosInsertRequest.UploadAsync();
                //}

                //if (this.lastUploadedVideo == null)
                //{
                //    throw new Exception("upload video error");
                //}

                //lesson.Video4 = this.lastUploadedVideo.Id;

                //// get video - to last when other will be awailable
                //var vReq = youTubeService.Videos.List("snippet");
                //vReq.Id = lesson.Video1;
                //var videoResp = vReq.Execute();
                //var video1 = videoResp.Items[0];

                //// update
                //// get full video, before 12 already exists
                //var title1 = FilesIOHelper.GetRndLine(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeName_1.txt");
                //title1 = title1
                //    .Replace("##lessonNum##", lessonNum.ToString())
                //    .Replace("##firsWordNum##", firsWordNum.ToString())
                //    .Replace("##lastWordNum##", lastWordNum.ToString())
                //    ;

                //var description1 = FilesIOHelper.GetTextFromFile(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeDescription_1.txt");
                //description1 = description1
                //    .Replace("##lessonNum##", lessonNum.ToString())
                //    .Replace("##firsWordNum##", firsWordNum.ToString())
                //    .Replace("##lastWordNum##", lastWordNum.ToString())
                //    .Replace("##YouTubeVideos_Part1##", youTubeVideos_Part1)
                //    .Replace("##YouTubeVideos_Part2##", youTubeVideos_Part2)
                //    .Replace("##YouTubeVideos_Part3##", youTubeVideos_Part3)
                //    .Replace("##YouTubeVideos_Part4##", youTubeVideos_Part4)
                //    .Replace("##CurrentWords##", currentWordsSB.ToString())
                //    .Replace("##ShortCurrentPhrases##", shortCurrentPhrasesSB.ToString())
                //    .Replace("##RepeatWords##", repeatWordsSB.ToString())
                //    .Replace("##ShortRepeatPhrases##", shortRepeatPhrasesSB.ToString())
                //    ;

                //video1.Snippet.Title = title1;
                //video1.Snippet.Description = description1;

                //var res = await youTubeService.Videos.Update(video1, "snippet").ExecuteAsync();

                //_context.Update(lesson);
                //_context.SaveChanges();
            }

            return Ok();
        }

        private async Task<YouTubeService> GetYouTubeService()
        {
            UserCredential credential;
            var inputFullPath = $"{FilesIOHelper.NeedFilesPath(_hostingEnvironment.WebRootPath)}\\client_secret.json";

            using (var stream = new FileStream(inputFullPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] { YouTubeService.Scope.Youtube },
                    "ddpoliglot@gmail.com",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            return youtubeService;
        }
        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:

                    Debug.Print("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Debug.Print("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        private Video lastUploadedVideo = null;

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Debug.Print("Video id '{0}' was successfully uploaded.", video.Id);
            this.lastUploadedVideo = video;
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }
    }
}

