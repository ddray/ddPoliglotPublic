using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ddPoliglotV6.BL.Enums;
using ddPoliglotV6.BL.Extentions;
using ddPoliglotV6.BL.Helpers;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using static ddPoliglotV6.BL.Managers.TranslationManager;

namespace ddPoliglotV6.Pages
{
    public class xIndex95_YouTubeProcess5Model : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;


        public xIndex95_YouTubeProcess5Model(
            ddPoliglotDbContext context,
            ILogger<IndexModel> logger,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
            )
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            UserManager = userManager;
        }

        public List<string> ResultStrs = new List<string>();

        public async Task OnGet()
        {
            //var words = _context.Words.OrderBy(x => x.Rate).ToList();
            //var cnt = 1;
            //foreach (var word in words)
            //{
            //    word.Rate = cnt++;
            //    _context.Update(word);
            //}

            //_context.SaveChanges();

            #region generate images for lessons
            //var startLesson = 1;
            //var endLesson = 11;
            //var baseName = "en_v1_lesson";

            //for (int lessonNum = startLesson; lessonNum <= endLesson; lessonNum++)
            //{
            //    var numString = lessonNum.ToString("#0000");
            //    var imageNumText = lessonNum.ToString();
            //    var imageName = $"{baseName}-{numString}_1.jpg";
            //    var imageNameThump = $"Thumb__{baseName}-{numString}_1.jpg";
            //    ImageHelper.GenerateLessonImages(imageName, imageNameThump, imageNumText, _hostingEnvironment.WebRootPath, _configuration);

            //    imageNumText = $"{lessonNum.ToString()}.1";
            //    imageName = $"{baseName}-{numString}_2.jpg";
            //    imageNameThump = $"Thumb__{baseName}-{numString}_2.jpg";
            //    ImageHelper.GenerateLessonImages(imageName, imageNameThump, imageNumText, _hostingEnvironment.WebRootPath, _configuration);

            //    imageNumText = $"{lessonNum.ToString()}.2";
            //    imageName = $"{baseName}-{numString}_3.jpg";
            //    imageNameThump = $"Thumb__{baseName}-{numString}_3.jpg";
            //    ImageHelper.GenerateLessonImages(imageName, imageNameThump, imageNumText, _hostingEnvironment.WebRootPath, _configuration);

            //    imageNumText = $"{lessonNum.ToString()}.3";
            //    imageName = $"{baseName}-{numString}_4.jpg";
            //    imageNameThump = $"Thumb__{baseName}-{numString}_4.jpg";
            //    ImageHelper.GenerateLessonImages(imageName, imageNameThump, imageNumText, _hostingEnvironment.WebRootPath, _configuration);
            //}



            #endregion generate images for lessons


            //UserCredential credential;
            //var inputFullPath = $"{FilesIOHelper.NeedFilesPath(_hostingEnvironment.WebRootPath)}\\client_secret.json";

            //using (var stream = new FileStream(inputFullPath, FileMode.Open, FileAccess.Read))
            //{
            //    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        // This OAuth 2.0 access scope allows for full read/write access to the
            //        // authenticated user's account.
            //        new[] { YouTubeService.Scope.Youtube },
            //        "ddpoliglot@gmail.com",
            //        CancellationToken.None,
            //        new FileDataStore(this.GetType().ToString())
            //    );
            //}

            //var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = this.GetType().ToString()
            //});

            //var channelsListRequest = youtubeService.Channels.List("contentDetails");
            //channelsListRequest.Mine = true;

            //// Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
            //var channelsListResponse = await channelsListRequest.ExecuteAsync();

            ////var playlistsR = youtubeService.Playlists.List("contentDetails");
            ////playlistsR.Mine = true;
            ////var playlistsResponse = await playlistsR.ExecuteAsync();

            ////foreach (var playList in playlistsResponse.Items)
            ////{
            ////}

            ////foreach (var channel in channelsListResponse.Items)
            ////{
            ////    // From the API response, extract the playlist ID that identifies the list
            ////    // of videos uploaded to the authenticated user's channel.
            ////    var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;

            ////    ResultStrs.Add($"Videos in list {uploadsListId}");

            ////    var nextPageToken = "";
            ////    while (nextPageToken != null)
            ////    {
            ////        var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
            ////        playlistItemsListRequest.PlaylistId = uploadsListId;
            ////        playlistItemsListRequest.MaxResults = 50;
            ////        playlistItemsListRequest.PageToken = nextPageToken;

            ////        // Retrieve the list of videos uploaded to the authenticated user's channel.
            ////        var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

            ////        foreach (var playlistItem in playlistItemsListResponse.Items)
            ////        {
            ////            // Print information about each video.
            ////            ResultStrs.Add($"{playlistItem.Snippet.Title} {playlistItem.Snippet.ResourceId.VideoId}");
            ////        }

            ////        nextPageToken = playlistItemsListResponse.NextPageToken;
            ////    }
            ////}

            //var title1 = GetRndLine(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeName_1.txt");
            //var description1 = GetTextFromFile(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\YouTubeDescription_1.txt");

            //var videoID = @"tW-fEp8TPVI";

            //var vReq = youtubeService.Videos.List("snippet");
            //vReq.Id = videoID;
            //var videoResp = vReq.Execute();
            //var video = videoResp.Items[0];
            //video.Snippet.Title = title1;
            //video.Snippet.Description = description1;

            //var res = await youtubeService.Videos.Update(video, "snippet").ExecuteAsync();



            //// add new video.
            ////var video = new Video();
            ////video.Snippet = new VideoSnippet();
            ////video.Snippet.Title = "Default Video Title";
            ////video.Snippet.Description = "Default Video Description";
            ////video.Snippet.Tags = new string[] { "Уроки английского языка", "Тренажер английского", "уровень английского языка A1" };
            ////video.Snippet.CategoryId = "27"; // education
            ////video.Status = new VideoStatus();
            ////video.Status.PrivacyStatus = "public"; // or "private" or "public"
            ////var filePath = @"D:\000_Work_7\ddPoliglot\files-repo-important\articles-video\en_v1_lesson0011_4__104_5098a3accc834d9c8e055497437c85b1.mp4"; // Replace with path to actual movie file.

            ////using (var fileStream = new FileStream(filePath, FileMode.Open))
            ////{
            ////    var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
            ////    videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
            ////    videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

            ////    await videosInsertRequest.UploadAsync();
            ////}
        }

        private string GetTextFromFile(string path)
        {
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);

            var sb = new StringBuilder();
            var line = "";
            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                sb.AppendLine(line);
            }

            file.Close();

            return sb.ToString();
        }

        private string GetRndLine(string path)
        {
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);

            var lines = new List<string>();
            var line = "";
            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                lines.Add(line);
            }

            file.Close();

            Random r = new Random();
            var index = r.Next(1, lines.Count());

            return lines[index];
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }
    }
}
