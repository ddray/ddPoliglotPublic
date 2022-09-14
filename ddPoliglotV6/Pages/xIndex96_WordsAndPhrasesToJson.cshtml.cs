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
using System.Text.Json;
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
    public class xIndex96_WordsAndPhrasesToJsonModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private WordManager _wordManager;


        public xIndex96_WordsAndPhrasesToJsonModel(
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
            _wordManager = new WordManager(_context, _configuration, _hostingEnvironment);
        }

        public List<string> ResultStrs = new List<string>();
        public string Result { get; set; }

        public async Task OnGet()
        {
            var spaSettings = new SpaAppSetting(
                new Language() { Code = "ru", LanguageID = 2 },
                new Language() { Code = "en", LanguageID = 1 }
                );

            var words = _wordManager.GetWordsWithFirstPhAndTr(spaSettings, 3, 1000, false);
            Result = JsonConvert.SerializeObject(words, Formatting.Indented);
        }
    }
}
