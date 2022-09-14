using ddPoliglotV6.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ddPoliglotV6.BL.Helpers
{
    class YouTubeHelper
    {
        ////private readonly ddPoliglotDbContext _context;
        ////private readonly IConfiguration _configuration;
        ////private readonly IHostingEnvironment _hostingEnvironment;
        ////private UserCredential credential { get; set; }

        ////public YouTubeHelper(ddPoliglotDbContext context,
        ////    IConfiguration configuration,
        ////    IHostingEnvironment hostingEnvironment
        ////    )
        ////{
        ////    _context = context;
        ////    _configuration = configuration;
        ////    _hostingEnvironment = hostingEnvironment;

        ////    var inputFullPath = $"{FilesIOHelper.NeedFilesPath(_hostingEnvironment.WebRootPath)}\\client_secret.json";

        ////}

        ////public async Task Init()
        ////{
        ////    using (var stream = new FileStream(inputFullPath, FileMode.Open, FileAccess.Read))
        ////    {
        ////        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        ////            GoogleClientSecrets.Load(stream).Secrets,
        ////                // This OAuth 2.0 access scope allows for full read/write access to the
        ////                // authenticated user's account.
        ////                new[] { YouTubeService.Scope.Youtube
        ////            },
        ////            "ddpoliglot@gmail.com",
        ////            CancellationToken.None,
        ////            new FileDataStore(this.GetType().ToString())
        ////        );
        ////    }
        ////}


        ////var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        ////    {
        ////        HttpClientInitializer = credential,
        ////        ApplicationName = this.GetType().ToString()
        ////    });
    }
}
