using ddPoliglotV6.BL.Extentions;
using ddPoliglotV6.BL.Helpers;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ddPoliglotV6.BL.Managers
{
    public class ArticleManager
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IConfiguration _configuration;

        public ArticleManager(ddPoliglotDbContext context,
            IConfiguration configuration
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _configuration = configuration;
        }

        public async Task<Article> GetFullById(int id)
        {
            var result = await _context.Articles.AsNoTracking().Where(x => x.ArticleID == id).AsNoTracking()
                .Include(y => y.ArticlePhrases).AsNoTracking()
                .Include(y => y.ArticleActors).AsNoTracking()
                .Include("ArticlePhrases.ArticleActor").AsNoTracking()
                .Include("ArticlePhrases.TrArticleActor").AsNoTracking()
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<Article> GetFullByIdShort(int id)
        {
            var result = await _context.Articles.Where(x => x.ArticleID == id)
                .Include(y => y.ArticlePhrases)
                .Include(y => y.ArticleActors)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task TextToSpeachArticlePhrase(int articlePhraseID, string rootPath)
        {
            var articlePhrase = await _context.ArticlePhrases.Where(x => x.ArticlePhraseID == articlePhraseID).Include(v => v.ArticleActor).Include(v => v.TrArticleActor).FirstOrDefaultAsync();

            if (TextToSpeachPhrase(articlePhrase, rootPath))
            {
                _context.Update(articlePhrase);
                await _context.SaveChangesAsync();
            }
        }

        public async Task TextToSpeachArticleAsync(TextToSpeechArticlePhraseArg args)
        {
            // add speach for all textes if need
            Article article = await _context.Articles.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ArticleID == args.ArticleID);
            List<ArticlePhrase> articlePhrases = await _context.ArticlePhrases.AsNoTracking()
                    .Where(x => x.ArticleID == args.ArticleID)
                    .AsNoTracking().Include(y => y.ArticleActor)
                    .AsNoTracking()
                    .Include(z => z.TrArticleActor).AsNoTracking()
                    .ToListAsync();

            foreach (var articlePhrase in articlePhrases)
            {
                if ((!args.SelectedArticlePhraseIDs.Contains(articlePhrase.ArticlePhraseID)
                    && args.SelectedArticlePhraseIDs.Count > 0)
                    || articlePhrase.Silent)
                {
                    continue;
                }

                if (TextToSpeachPhrase(articlePhrase, args.BaseRootPath))
                {
                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {

                        ent.Update(articlePhrase);
                        await ent.SaveChangesAsync();
                    }
                }
            }

            var files = new List<string>();

            // concat all audios with pauses
            foreach (var articlePhrase in articlePhrases.OrderBy(x => x.OrderNum))
            {
                if (!args.SelectedArticlePhraseIDs.Contains(articlePhrase.ArticlePhraseID)
                    && args.SelectedArticlePhraseIDs.Count > 0
                    || articlePhrase.Silent)
                {
                    continue;
                }

                if (articlePhrase.ActivityType == 0 || articlePhrase.ActivityType == 2)
                {
                    // source text first
                    if (!string.IsNullOrEmpty(articlePhrase.Text))
                    {
                        files.Add(articlePhrase.TextSpeechFileName);
                        AudioHelper.AddPauseRepeat(files, articlePhrase.Pause);
                    }

                    // translation text second
                    if (!string.IsNullOrEmpty(articlePhrase.TrText))
                    {
                        files.Add(articlePhrase.TrTextSpeechFileName);
                        AudioHelper.AddPauseRepeat(files, articlePhrase.TrPause);
                    }
                }
                else
                {
                    // translation text
                    if (!string.IsNullOrEmpty(articlePhrase.TrText))
                    {
                        files.Add(articlePhrase.TrTextSpeechFileName);
                        AudioHelper.AddPauseRepeat(files, articlePhrase.TrPause);
                    }

                    // source text
                    if (!string.IsNullOrEmpty(articlePhrase.Text))
                    {
                        files.Add(articlePhrase.TextSpeechFileName);
                        AudioHelper.AddPauseRepeat(files, articlePhrase.Pause);
                    }
                }
            }

            var filename = ($"{article.Name.Trim()}__{args.ArticleID.Value}_{Guid.NewGuid()}.mp3").Replace("-", "").Replace(" ", "-");

            (new FilesIOHelper(_configuration)).ComposeAudioAndSave(filename, files, args.BaseRootPath);

            article.TextSpeechFileName = filename;
            _context.Update(article);
            _context.SaveChanges();
        }

        public bool TextToSpeachPhrase(ArticlePhrase articlePhrase, string rootPath)
        {
            var changed = false;

            if (string.IsNullOrWhiteSpace(articlePhrase.Text)
                && !string.IsNullOrWhiteSpace(articlePhrase.TextSpeechFileName))
            {
                // text empty but speach exists
                articlePhrase.TextSpeechFileName = "";
                articlePhrase.SpeachDuration = 0;
                articlePhrase.HashCode = 0;
                changed = true;
            }
            else
            {
                var oldPhraseSpeachHashCode = HashManager.GetHashCodeFromFileName(articlePhrase.TextSpeechFileName);

                // calc new hash
                articlePhrase.HashCode = articlePhrase.GetPhraseHash();

                if (!string.IsNullOrWhiteSpace(articlePhrase.Text) // text exists
                    && oldPhraseSpeachHashCode != articlePhrase.HashCode // and file is empty or wrong for this phrase
                    )
                {
                    var duration = 0;
                    var filename = "";

                    if (articlePhrase.Text.Contains("?"))
                    {
                        int a = 1;
                    }

                    var speechFile = _context.SpeechFiles.FirstOrDefault(x => x.HashCode == articlePhrase.HashCode);

                    if (speechFile == null)
                    {
                        // there is no speach with such text before
                        var credential = GoogleCredential.FromFile(FilesIOHelper.TranslationKeyFileName(rootPath))
                            .CreateScoped(TextToSpeechClient.DefaultScopes);
                        var channel = new Grpc.Core.Channel(TextToSpeechClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
                        var clientTTS = TextToSpeechClient.Create(channel);

                        var resultSsml = AudioHelper.TextToSsml(articlePhrase.Text);

                        var response = clientTTS.SynthesizeSpeech(new SynthesizeSpeechRequest
                        {
                            Input = new SynthesisInput
                            {
                                Ssml = resultSsml
                            },
                            Voice = new VoiceSelectionParams
                            {
                                LanguageCode = articlePhrase.ArticleActor.Language.ToLower() == "en"
                                ? "en-US"
                                : $"{articlePhrase.ArticleActor.Language.ToLower()}-{articlePhrase.ArticleActor.Language.ToUpper()}",
                                Name = articlePhrase.ArticleActor.VoiceName
                            },
                            AudioConfig = new AudioConfig
                            {
                                AudioEncoding = AudioEncoding.Mp3,
                                SpeakingRate = (double)articlePhrase.ArticleActor.VoiceSpeed,
                                Pitch = (double)articlePhrase.ArticleActor.VoicePitch
                            }
                        });

                        duration = AudioHelper.GetAudioDuration(response.AudioContent.ToByteArray());
                        filename = $"{articlePhrase.HashCode.ToString().Replace("-", "n")}_{duration}_phrase.mp3";

                        (new FilesIOHelper(_configuration))
                            .SavePhraseAudio(filename, response.AudioContent.ToByteArray(), rootPath);

                        _context.SpeechFiles.Add(new SpeechFile()
                        {
                            Duration = duration,
                            HashCode = articlePhrase.HashCode,
                            SpeechFileName = filename,
                            Version = 1
                        });

                        _context.SaveChanges();
                    }
                    else
                    {
                        // speach for text with such hash already exists
                        filename = speechFile.SpeechFileName;
                        duration = speechFile.Duration;
                    }

                    articlePhrase.TextSpeechFileName = filename;
                    articlePhrase.SpeachDuration = duration;

                    changed = true;
                }
            }

            if (string.IsNullOrWhiteSpace(articlePhrase.TrText)
                && !string.IsNullOrWhiteSpace(articlePhrase.TrTextSpeechFileName))
            {
                // text empty but speach exists
                articlePhrase.TrTextSpeechFileName = "";
                articlePhrase.TrSpeachDuration = 0;
                articlePhrase.TrHashCode = 0;
                changed = true;
            }
            else
            {
                var oldPhraseSpeachHashCode = HashManager.GetHashCodeFromFileName(articlePhrase.TrTextSpeechFileName);

                // calc new hash
                articlePhrase.TrHashCode = articlePhrase.GetTrPhraseHash();

                if (!string.IsNullOrWhiteSpace(articlePhrase.TrText) // text exists
                    && oldPhraseSpeachHashCode != articlePhrase.TrHashCode // and file is empty or wrong for this phrase
                    )
                {
                    var duration = 0;
                    var filename = "";

                    var speechFile = _context.SpeechFiles.FirstOrDefault(x => x.HashCode == articlePhrase.TrHashCode);

                    if (speechFile == null)
                    {
                        // there is no speach with such text before
                        var credential = GoogleCredential.FromFile(FilesIOHelper.TranslationKeyFileName(rootPath))
                            .CreateScoped(TextToSpeechClient.DefaultScopes);
                        var channel = new Grpc.Core.Channel(TextToSpeechClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
                        var clientTTS = TextToSpeechClient.Create(channel);

                        var response = clientTTS.SynthesizeSpeech(new SynthesizeSpeechRequest
                        {
                            Input = new SynthesisInput
                            {
                                Ssml = AudioHelper.TextToSsml(articlePhrase.TrText)
                            },
                            Voice = new VoiceSelectionParams
                            {
                                LanguageCode = articlePhrase.TrArticleActor.Language.ToLower() == "en"
                                ? "en-US"
                                : $"{articlePhrase.TrArticleActor.Language.ToLower()}-{articlePhrase.TrArticleActor.Language.ToUpper()}",
                                Name = articlePhrase.TrArticleActor.VoiceName
                            },
                            AudioConfig = new AudioConfig
                            {
                                AudioEncoding = AudioEncoding.Mp3,
                                SpeakingRate = (double)articlePhrase.TrArticleActor.VoiceSpeed,
                                Pitch = (double)articlePhrase.TrArticleActor.VoicePitch
                            }
                        });

                        duration = AudioHelper.GetAudioDuration(response.AudioContent.ToByteArray());
                        filename = $"{articlePhrase.TrHashCode.ToString().Replace("-", "n")}_{duration}_phrase.mp3";

                        (new FilesIOHelper(_configuration))
                            .SavePhraseAudio(filename, response.AudioContent.ToByteArray(), rootPath);

                        _context.SpeechFiles.Add(new SpeechFile()
                        {
                            Duration = duration,
                            HashCode = articlePhrase.TrHashCode,
                            SpeechFileName = filename,
                            Version = 1
                        });

                        _context.SaveChanges();
                    }
                    else
                    {
                        // speach for text with such hash already exists
                        filename = speechFile.SpeechFileName;
                        duration = speechFile.Duration;
                    }

                    articlePhrase.TrTextSpeechFileName = filename;
                    articlePhrase.TrSpeachDuration = duration;

                    changed = true;
                }
            }

            return changed;
        }

        public static string TrimEx(string text, ArticlePhrase articlePhrase)
        {
            if (!string.IsNullOrEmpty(articlePhrase.ChildrenType))
            {
                // mixed children
                return text.Trim(new char[] { ' ', ',', '.', ':', ';', '!', ' ' });
            }
            else
            {
                // added by hand
                return text.Trim(new char[] { ' ', ',' });
            }
        }

        public async Task TextToVideoArticle(TextToSpeechArticlePhraseArg args)
        {
            Article article = null;
            List<ArticlePhrase> articlePhrases = null;
            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                article = await ent.Articles.AsNoTracking().FirstOrDefaultAsync(x => x.ArticleID == args.ArticleID); await ent.SaveChangesAsync();
                articlePhrases = await _context.ArticlePhrases.AsNoTracking().Where(x => x.ArticleID == args.ArticleID
                               && !x.Silent
                               && (x.Text != "" || x.TrText != ""
                               )).OrderBy(x => x.OrderNum)
                               .AsNoTracking().Include(y => y.ArticleActor).AsNoTracking()
                               .Include(z => z.TrArticleActor).AsNoTracking()
                               .ToListAsync();
            }


            if (args.SelectedArticlePhraseIDs.Count > 0)
            {
                // filter selected
                articlePhrases = articlePhrases.Where(x => args.SelectedArticlePhraseIDs.Contains(x.ArticlePhraseID)).OrderBy(x => x.OrderNum).ToList();
            }

            var articleManager = new ArticleManager(_context, _configuration);
            // add speach for all textes if need
            foreach (var articlePhrase in articlePhrases)
            {
                if (articleManager.TextToSpeachPhrase(articlePhrase, args.BaseRootPath))
                {
                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        ent.Update(articlePhrase);
                        await ent.SaveChangesAsync();
                    }
                }
            }

            var filename = ($"{article.Name}__{args.ArticleID.Value}_{Guid.NewGuid()}.mp4").Replace("-", "").Replace(" ", "-");

            (new FilesIOHelper(_configuration)).CombineAndSaveVideo(args.BaseRootPath, article, articlePhrases, filename);


            article.VideoFileName = filename;
            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                ent.Update(article);
                await ent.SaveChangesAsync();
            }
        }
    }
}
