using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data.Models;
using ddPoliglotV6.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System;
using ddPoliglotV6.BL.Helpers;

namespace ddPoliglotV6.Pages
{
    public class xIndex97_WordsAndPhrasesCheckIfAudioExixtsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private WordManager _wordManager;


        public xIndex97_WordsAndPhrasesCheckIfAudioExixtsModel(
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
            foreach (var word in words)
            {
                CheckAudio($"word rate: {word.Rate}", word.TextSpeechFileName);
                CheckAudio($"word rate: {word.Rate} speed 1", word.TextSpeechFileNameSpeed1);
                CheckAudio($"word rate: {word.Rate} speed 2", word.TextSpeechFileNameSpeed2);
                foreach (var wordPhrase in word.WordPhraseSelected)
                {
                    CheckAudio($"word rate: {word.Rate}, ph id: {wordPhrase.WordPhraseID}", word.TextSpeechFileName);
                    CheckAudio($"word rate: {word.Rate}, ph id: {wordPhrase.WordPhraseID}  speed 1", word.TextSpeechFileNameSpeed1);
                    CheckAudio($"word rate: {word.Rate}, ph id: {wordPhrase.WordPhraseID}  speed 2", word.TextSpeechFileNameSpeed2);
                }
            }
        }

        private void CheckAudio(string pref, string textSpeechFileName)
        {
            if (String.IsNullOrWhiteSpace(textSpeechFileName))
            {
                return;
            }

            var audioPath = FilesIOHelper.GetPhrasesAudioFolder(_hostingEnvironment.WebRootPath, _configuration);
            if (!System.IO.File.Exists($"{audioPath}/{textSpeechFileName}"))
            {
                ResultStrs.Add($"{textSpeechFileName} /// {pref}");
            }
        }
    }
}
