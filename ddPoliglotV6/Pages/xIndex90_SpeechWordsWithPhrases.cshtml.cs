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
    public class xIndex90_SpeechWordsWithPhrasesModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private TranslationManager _translationManager;
        private WordManager _wordManager;


        public xIndex90_SpeechWordsWithPhrasesModel(
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
            _translationManager = new TranslationManager(_context, _configuration, _hostingEnvironment);
            _wordManager = new WordManager(_context, _configuration, _hostingEnvironment);
        }

        public List<string> ResultStrs = new List<string>();

        public async Task OnGet()
        {
            // var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var spaAppSetting = new SpaAppSetting(
                new Language() { Code = "ru", LanguageID = 2 },
                new Language() { Code = "en", LanguageID = 1 }
                );

            var words = _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                .Where(x => x.Rate >= 700 && x.Rate <= 1000)
                .OrderBy(x=>x.Rate)
                .ToList();
            var nativeLanguageId = spaAppSetting.NativeLanguage?.LanguageID ?? 2;
            foreach (var oldItem in words)
            {
                _wordManager.SpeechForWordUpdate(oldItem);

                var wordTranslation = _context.WordTranslations.AsNoTracking().Where(x => x.WordID == oldItem.WordID
                    && x.LanguageID == nativeLanguageId).FirstOrDefault();

                if (wordTranslation != null)
                {
                    _wordManager.SpeechForWordTranslationUpdate(wordTranslation);
                }

                var wordPhrasies = (from wpw in _context.WordPhraseWords.AsNoTracking()
                                    join wp in _context.WordPhrases.AsNoTracking() on wpw.WordPhraseID equals wp.WordPhraseID
                                    join wpt in _context.WordPhraseTranslations.AsNoTracking() on new { wp.WordPhraseID, LanguageID = nativeLanguageId } equals new { wpt.WordPhraseID, wpt.LanguageID } into grouping1
                                    from wptj in grouping1.DefaultIfEmpty()
                                    where wpw.WordID == oldItem.WordID && wpw.PhraseOrder > 0
                                    select new WordPhrase()
                                    {
                                        LanguageID = wp.LanguageID,
                                        Text = wp.Text,
                                        HashCode = wp.HashCode,
                                        HashCodeSpeed1 = wp.HashCodeSpeed1,
                                        HashCodeSpeed2 = wp.HashCodeSpeed2,
                                        SpeachDuration = wp.SpeachDuration,
                                        SpeachDurationSpeed1 = wp.SpeachDurationSpeed1,
                                        SpeachDurationSpeed2 = wp.SpeachDurationSpeed2,
                                        TextSpeechFileName = wp.TextSpeechFileName,
                                        TextSpeechFileNameSpeed1 = wp.TextSpeechFileNameSpeed1,
                                        TextSpeechFileNameSpeed2 = wp.TextSpeechFileNameSpeed2,
                                        WordsUsed = wp.WordsUsed,
                                        WordPhraseID = wp.WordPhraseID,
                                        WordPhraseTranslation = wptj,
                                        PhraseOrderInCurrentWord = wpw.PhraseOrder,
                                        CurrentWordID = wpw.WordID
                                    }).OrderByDescending(x=>x.PhraseOrderInCurrentWord).Take(3).ToList();

                foreach (var wordPhrase in wordPhrasies)
                {
                    _wordManager.SpeechForWordPhraseUpdate(wordPhrase);
                    if (wordPhrase.WordPhraseTranslation != null)
                    {
                        _wordManager.SpeechForWordPhraseTranslationUpdate(wordPhrase.WordPhraseTranslation);
                    }
                }

                _wordManager.UpdateWordPhrasiesStateForLessons(oldItem.WordID, spaAppSetting);
            }
        }
    }
}
