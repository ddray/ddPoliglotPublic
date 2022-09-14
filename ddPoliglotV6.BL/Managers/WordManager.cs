using ddPoliglotV6.BL.Extentions;
using ddPoliglotV6.BL.Helpers;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ddPoliglotV6.BL.Managers
{
    public class WordManager
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public WordManager(ddPoliglotDbContext context,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public bool TextToSpeachWord(Word word)
        {

            return false;
        }

        public void SpeechForWordUpdate(Word oldItem)
        {
            if (oldItem == null)
            {
                return;
            }

            var voice = AudioHelper.GetDefaultVoiceByLanguage(oldItem.LanguageID);

            var text = AddPronIfNeed(oldItem.Text, oldItem.Pronunciation);

            // generate
            var speechData = AudioHelper.MakeSpeechIfNeed(
                _context,
                _configuration,
                voice,
                rootPath: _hostingEnvironment.WebRootPath,
                text,
                oldItem.HashCode,
                oldItem.TextSpeechFileName,
                oldItem.SpeachDuration,
                null
                );

            if (speechData.Changed)
            {
                oldItem.HashCode = speechData.HashCode;
                oldItem.TextSpeechFileName = speechData.TextSpeechFileName;
                oldItem.SpeachDuration = speechData.SpeachDuration;
            }

            var textSpeed1 = AudioHelper.formatTextAsSlowAudio(text, 1);
            var speechDataSpeed1 = AudioHelper.MakeSpeechIfNeed(
                _context,
                _configuration,
                voice,
                rootPath: _hostingEnvironment.WebRootPath,
                textSpeed1,
                oldItem.HashCodeSpeed1,
                oldItem.TextSpeechFileNameSpeed1,
                oldItem.SpeachDurationSpeed1,
                null
                );

            if (speechDataSpeed1.Changed)
            {
                oldItem.HashCodeSpeed1 = speechDataSpeed1.HashCode;
                oldItem.TextSpeechFileNameSpeed1 = speechDataSpeed1.TextSpeechFileName;
                oldItem.SpeachDurationSpeed1 = speechDataSpeed1.SpeachDuration;
            }

            var textSpeed2 = AudioHelper.formatTextAsSlowAudio(text, 2);
            var speechDataSpeed2 = AudioHelper.MakeSpeechIfNeed(
                _context,
                _configuration,
                voice,
                rootPath: _hostingEnvironment.WebRootPath,
                textSpeed2,
                oldItem.HashCodeSpeed2,
                oldItem.TextSpeechFileNameSpeed2,
                oldItem.SpeachDurationSpeed2,
                null
                );

            if (speechDataSpeed1.Changed)
            {
                oldItem.HashCodeSpeed2 = speechDataSpeed2.HashCode;
                oldItem.TextSpeechFileNameSpeed2 = speechDataSpeed2.TextSpeechFileName;
                oldItem.SpeachDurationSpeed2 = speechDataSpeed2.SpeachDuration;
            }

            if (speechData.Changed || speechDataSpeed1.Changed || speechDataSpeed2.Changed)
            {
                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    ent.Update(oldItem);
                    ent.SaveChanges();
                }
            }
        }

        private string AddPronIfNeed(string text, string pronunciation)
        {
            var result = text;
            if (!String.IsNullOrEmpty(pronunciation) && text.IndexOf("Pron") < 0)
            {
                result = $"{text} {{\"Pron\":\"{pronunciation}\"}}";
            }

            return result;
        }

        public void SpeechForWordTranslationUpdate(WordTranslation wordTranslation)
        {
            if (wordTranslation == null)
            {
                return;
            }

            var voice = AudioHelper.GetDefaultVoiceByLanguage(wordTranslation.LanguageID);

            // generate
            var speechData = AudioHelper.MakeSpeechIfNeed(
                _context,
                _configuration,
                voice,
                rootPath: _hostingEnvironment.WebRootPath,
                wordTranslation.Text,
                wordTranslation.HashCode,
                wordTranslation.TextSpeechFileName,
                wordTranslation.SpeachDuration,
                null
                );

            if (speechData.Changed)
            {
                wordTranslation.HashCode = speechData.HashCode;
                wordTranslation.TextSpeechFileName = speechData.TextSpeechFileName;
                wordTranslation.SpeachDuration = speechData.SpeachDuration;

                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    ent.Update(wordTranslation);
                    ent.SaveChanges();
                }
            }
        }

        public void SpeechForWordPhraseUpdate(WordPhrase wordPhrase)
        {
            if (wordPhrase == null)
            {
                return;
            }

            var voice = AudioHelper.GetDefaultVoiceByLanguage(wordPhrase.LanguageID);

            var text = wordPhrase.Text.Trim().TrimEnd('.').TrimEnd('!').TrimEnd(',');
            // generate
            var speechData = AudioHelper.MakeSpeechIfNeed(
                _context,
                _configuration,
                voice,
                rootPath: _hostingEnvironment.WebRootPath,
                text,
                wordPhrase.HashCode,
                wordPhrase.TextSpeechFileName,
                wordPhrase.SpeachDuration,
                null
                );

            if (speechData.Changed)
            {
                wordPhrase.HashCode = speechData.HashCode;
                wordPhrase.TextSpeechFileName = speechData.TextSpeechFileName;
                wordPhrase.SpeachDuration = speechData.SpeachDuration;

            }
            
            var textSpeed1 = AudioHelper.formatTextAsSlowAudio(text, 1);
            var speechDataSpeed1 = AudioHelper.MakeSpeechIfNeed(
                _context,
                _configuration,
                voice,
                rootPath: _hostingEnvironment.WebRootPath,
                textSpeed1,
                wordPhrase.HashCodeSpeed1,
                wordPhrase.TextSpeechFileNameSpeed1,
                wordPhrase.SpeachDurationSpeed1,
                null
                );

            if (speechDataSpeed1.Changed)
            {
                wordPhrase.HashCodeSpeed1 = speechDataSpeed1.HashCode;
                wordPhrase.TextSpeechFileNameSpeed1 = speechDataSpeed1.TextSpeechFileName;
                wordPhrase.SpeachDurationSpeed1 = speechDataSpeed1.SpeachDuration;
            }

            var textSpeed2 = AudioHelper.formatTextAsSlowAudio(text, 2);
            var speechDataSpeed2 = AudioHelper.MakeSpeechIfNeed(
                _context,
                _configuration,
                voice,
                rootPath: _hostingEnvironment.WebRootPath,
                textSpeed2,
                wordPhrase.HashCodeSpeed2,
                wordPhrase.TextSpeechFileNameSpeed2,
                wordPhrase.SpeachDurationSpeed2,
                null
                );

            if (speechDataSpeed1.Changed)
            {
                wordPhrase.HashCodeSpeed2 = speechDataSpeed2.HashCode;
                wordPhrase.TextSpeechFileNameSpeed2 = speechDataSpeed2.TextSpeechFileName;
                wordPhrase.SpeachDurationSpeed2 = speechDataSpeed2.SpeachDuration;
            }

            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                ent.Update(wordPhrase);
                ent.SaveChanges();
            }
        }

        public void SpeechForWordPhraseTranslationUpdate(WordPhraseTranslation wordPhraseTranslation)
        {
            if (wordPhraseTranslation == null)
            {
                return;
            }

            var voice = AudioHelper.GetDefaultVoiceByLanguage(wordPhraseTranslation.LanguageID);

            var text = wordPhraseTranslation.Text.Trim().TrimEnd('.').TrimEnd('!').TrimEnd(',');
            // generate
            var speechData = AudioHelper.MakeSpeechIfNeed(
                _context,
                _configuration,
                voice,
                rootPath: _hostingEnvironment.WebRootPath,
                text,
                wordPhraseTranslation.HashCode,
                wordPhraseTranslation.TextSpeechFileName,
                wordPhraseTranslation.SpeachDuration,
                null
                );

            if (speechData.Changed)
            {
                wordPhraseTranslation.HashCode = speechData.HashCode;
                wordPhraseTranslation.TextSpeechFileName = speechData.TextSpeechFileName;
                wordPhraseTranslation.SpeachDuration = speechData.SpeachDuration;

                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    ent.Update(wordPhraseTranslation);
                    ent.SaveChanges();
                }
            }
        }

        public int UpdateWordPhrasiesStateForLessons(int wordID, SpaAppSetting spaAppSetting)
        {
            var nativeLanguage = spaAppSetting.NativeLanguage?.LanguageID ?? 2;
            // get phrases ready foo lessons
            var readyForLessonPhrasiesCnt = (
                from wpw in _context.WordPhraseWords.AsNoTracking()
                join wp in _context.WordPhrases.AsNoTracking() on wpw.WordPhraseID equals wp.WordPhraseID
                join wpt in _context.WordPhraseTranslations on new
                {
                    wp.WordPhraseID,
                    LanguageID = nativeLanguage
                }
                equals new
                {
                    wpt.WordPhraseID,
                    wpt.LanguageID
                }
                into grouping1
                from wptj in grouping1.DefaultIfEmpty()
                where 
                    wpw.WordID == wordID 
                    && wpw.PhraseOrder > 0 // ordered
                    && (wp.TextSpeechFileName ?? "") != "" // phrase speeched
                    && (
                        wptj != null 
                        && (wptj.LanguageID == nativeLanguage) 
                        && (wptj.TextSpeechFileName ?? "") != ""
                        ) // phrase translation speeched
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
                }).Count();

            var wordTranslation = _context.WordTranslations.FirstOrDefault(x=>x.WordID == wordID && x.LanguageID == nativeLanguage);
            if (wordTranslation != null && wordTranslation.ReadyForLessonPhrasiesCnt != readyForLessonPhrasiesCnt)
            {
                wordTranslation.ReadyForLessonPhrasiesCnt = readyForLessonPhrasiesCnt;
                _context.Update(wordTranslation);
                _context.SaveChanges();
            }

            return readyForLessonPhrasiesCnt;
        }

        public void FillWordListWithPhrases(List<Word> resData, SpaAppSetting spaAppSetting, bool readyOnly = false)
        {
            var wordIDs = resData.Select(x => x.WordID).ToList();
            var query = 
                from wpw in _context.WordPhraseWords.AsNoTracking()
                join wp in _context.WordPhrases.AsNoTracking() on wpw.WordPhraseID equals wp.WordPhraseID
                join wpt in _context.WordPhraseTranslations on new
                {
                    wp.WordPhraseID,
                    spaAppSetting.NativeLanguage.LanguageID
                }
                equals new
                {
                    wpt.WordPhraseID,
                    wpt.LanguageID
                }
                into grouping1
                from wptj in grouping1.DefaultIfEmpty()
                where wordIDs.Contains(wpw.WordID) && wpw.PhraseOrder > 0
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
                    WordPhraseID = wp.WordPhraseID,
                    WordPhraseTranslation = wptj,
                    PhraseOrderInCurrentWord = wpw.PhraseOrder,
                    CurrentWordID = wpw.WordID
                };

            if (readyOnly)
            {
                query = query.Where(x => 
                x.PhraseOrderInCurrentWord > 0 
                && (x.TextSpeechFileName ?? "") != ""
                && (x.WordPhraseTranslation != null && (x.WordPhraseTranslation.TextSpeechFileName ?? "") != "")
                );
            }

            var wordPhrases = query.ToList();

            foreach (var word in resData)
            {
                word.WordPhraseSelected = wordPhrases.Where(x => x.CurrentWordID == word.WordID).OrderByDescending(x=>x.PhraseOrderInCurrentWord).ToList();
            }
        }

        public async Task<Word> CreateWordWithTransAndPronAsync(string text, SpaAppSetting spaAppSetting, int rate)
        {
            var pron = "";
            var translation = (new TranslationManager(_context, _configuration, _hostingEnvironment))
                .Translate(new TranslateArg() 
                {  
                    SourceLanguage = spaAppSetting.LearnLanguage.Code,
                    TargetLanguage = spaAppSetting.NativeLanguage.Code,
                    SourceText = text
                });

            if (spaAppSetting.LearnLanguage.Code == "en")
            {
                var results = await TranslationManager.GetDataYandex(text);
                var result = string.Join(",", results.Phrases);

                if (result.Length > 240)
                {
                    result = result.Substring(0, 230);
                }

                pron = results.Pronunciation;
            }

            var word = new Word()
            {
                Text = text.Trim(),
                LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                Pronunciation = pron,
                Rate = rate,
            };

            _context.Add(word);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(translation))
            {
                var wordTranslation = new WordTranslation()
                {
                    WordID = word.WordID,
                    LanguageID = spaAppSetting.NativeLanguage.LanguageID,
                    Text = translation,
                };

                _context.Add(wordTranslation);
                _context.SaveChanges();
            }

            return word;
        }

        public async Task AddListFromTextWithPhrases(string sourceText, SpaAppSetting spaAppSetting, bool withMix = false)
        {
            var allWords = _context.Words.Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID).ToList();
            var words = new List<Word>();
            Word currWord = null;
            var currWordPhrase = new List<WordPhrase>();
            var curentRate = -100000;
            var arr = sourceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var lines = new List<string>();
            var wordsQty = 0;
            foreach (var line in arr)
            {
                if(string.IsNullOrEmpty(line.Trim()))
                    continue;

                if (line.Trim().StartsWith("*"))
                {
                    var ar = line.Split(";");
                    var text = ar[0].Trim().TrimStart('*');
                    curentRate = Convert.ToInt32(text);
                }
                else
                {
                    if (line.Trim().StartsWith("+"))
                    {
                        wordsQty++;
                    }

                    lines.Add(line.Trim());
                }
            }

            if (curentRate == -100000)
            {
                curentRate = allWords.Count() > 0 ? allWords.Max(x=>x.Rate) :0;
            }

            // prepare rnd rates if need
            var rates = Enumerable.Range(curentRate, (curentRate + wordsQty)).ToList();

            if (withMix)
            {
                Random rnd = new Random();
                rates = rates.OrderBy(x => rnd.Next()).ToList();
            }

            var rateIndex = 0;
            foreach (var item in lines)
            {
                if (item.StartsWith("+"))
                {
                    // word
                    var ar = item.Split(";");
                    var text = ar[0].TrimStart('+').Trim().FirstCharToUpper();
                    currWord = allWords.FirstOrDefault(x => x.Text == text);
                    if (currWord == null)
                    {
                        currWord = await CreateWordWithTransAndPronAsync(text, spaAppSetting, rates[rateIndex]);
                        allWords.Add(currWord);
                    }
                    else
                    {
                        // use existing word
                        currWord.Rate = rates[rateIndex];
                        currWord.Deleted = false;
                        _context.Update<Word>(currWord);
                        _context.SaveChanges();
                    }

                    rateIndex++;
                }
                else if (item.StartsWith("-"))
                {
                    // phrase
                    var ar = item.Split(";");
                    var text = ar[0].TrimStart('-').Trim().FirstCharToUpper().LastCharToDot();
                    WordPhrase wordPhrase = new WordPhrase()
                    {
                        LanguageID = (int)spaAppSetting.LearnLanguage.LanguageID,
                        Text = text
                    };

                    wordPhrase.HashCode = wordPhrase.GetHashCode();
                    WordPhraseWord wordPhraseWord = new WordPhraseWord()
                    {
                        WordID = currWord.WordID,
                        WordPhrase = wordPhrase,
                        PhraseOrder = 1000
                    };

                    wordPhrase.WordPhraseWords.Add(wordPhraseWord);
                    currWord.WordPhraseWords.Add(wordPhraseWord);
                    _context.WordPhrases.Add(wordPhrase);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("not formated");
                }
            }
        }

        public async Task AddListFromTextWithPhrasesV3(string sourceText, SpaAppSetting spaAppSetting, bool withMix = false)
        {
            var allWords = _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID).ToList();
            var arr = sourceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            Word currWord = null;
            var startOrder = 2800;
            var phOrder = startOrder;
            foreach (var line in arr)
            {
                if (line.StartsWith("++"))
                {
                    phOrder = startOrder;
                    // word
                    var ar = line.Split((new string[] { "++", "///" }) as string[], StringSplitOptions.RemoveEmptyEntries);
                    var needToDelete = false;
                    if (ar[0].StartsWith("--"))
                    {
                        needToDelete = true;
                        ar[0] = ar[0].Replace("--", "");
                    }

                    var wordText = ar[0].Trim().FirstCharToUpper();
                    currWord = allWords.FirstOrDefault(x => x.TextClear.ToLower() == wordText.ToLower() || x.Text.ToLower() == wordText.ToLower());
                    if (currWord == null)
                    {
                        Debug.Print($"!!!!!!!!!!!!!!!!!!!!!!!!! no word {wordText}");
                        continue;
                    }

                    // translation

                    if (ar.Length == 1)
                    {
                        Debug.Print($"!!!!!!!!!!!!!!!!!!!!!!!!! no word translation {wordText}");
                        continue;
                    }

                    var wTranslation = ar[1].Trim().ToLower();
                    if (string.IsNullOrEmpty(wTranslation))
                    {
                        Debug.Print($"!!!!!!!!!!!!!!!!!!!!!!!!! no word translation {wordText}");
                        continue;
                    }

                    var wordTranslation = _context.WordTranslations
                        .FirstOrDefault(x =>
                            x.LanguageID == spaAppSetting.NativeLanguage.LanguageID
                            && x.WordID == currWord.WordID
                            );

                    if (wordTranslation == null)
                    {
                        wordTranslation = new WordTranslation()
                        {
                            WordID = currWord.WordID,
                            LanguageID = spaAppSetting.NativeLanguage.LanguageID,
                            Text = wTranslation,
                        };

                        _context.Add(wordTranslation);
                    }
                    else
                    {
                        wordTranslation.Text = wTranslation;
                        _context.Update(wordTranslation);
                    }

                    if (needToDelete)
                    {
                        currWord.Deleted = true;
                        _context.Update(currWord);
                    }

                    _context.SaveChanges();
                }
                else if (line.Trim().StartsWith("##"))
                {
                    // phrase
                    var ar = line.Trim().Split((new string[] { "##", "///" }) as string[], StringSplitOptions.RemoveEmptyEntries);
                    var needToDelete = false;
                    if (ar[0].StartsWith("--"))
                    {
                        needToDelete = true;
                        ar[0] = ar[0].Replace("--", "");
                    }

                    var text = ar[0].Trim().FirstCharToUpper().LastCharToDot();

                    WordPhrase wordPhrase;
                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        wordPhrase = ent.WordPhrases.AsNoTracking().FirstOrDefault(x => x.Text == text);
                        if (wordPhrase == null)
                        {
                            wordPhrase = new WordPhrase()
                            {
                                LanguageID = (int)spaAppSetting.LearnLanguage.LanguageID,
                                Text = text
                            };
                        }
                        else
                        {
                            wordPhrase.Text = text;
                        }

                        wordPhrase.HashCode = wordPhrase.GetHashCode();

                        if (wordPhrase.WordPhraseID > 0)
                        {
                            ent.Update(wordPhrase);
                        }
                        else
                        {
                            ent.WordPhrases.Add(wordPhrase);
                        }

                        var wordPhraseWord = ent.WordPhraseWords.AsNoTracking()
                            .FirstOrDefault(x =>
                                x.WordID == currWord.WordID
                                && x.WordPhraseID == wordPhrase.WordPhraseID
                            );

                        if (wordPhraseWord == null)
                        {
                            wordPhraseWord = new WordPhraseWord()
                            {
                                WordID = currWord.WordID,
                                WordPhrase = wordPhrase,
                                PhraseOrder =  needToDelete ? -1 : phOrder--
                            };

                            wordPhrase.WordPhraseWords.Add(wordPhraseWord);
                            currWord.WordPhraseWords.Add(wordPhraseWord);
                        }
                        else
                        {
                            wordPhraseWord.PhraseOrder = needToDelete ? -1 : phOrder--;
                            ent.Update(wordPhraseWord);
                        }

                        ent.SaveChanges();

                        if (ar.Length == 1)
                        {
                            Debug.Print($"!!!!!!!!!!!!!!!!!!!!!!!!! no wordPhrase translation {text}");
                            continue;
                        }

                        var wordPhraseTranslation = ent.WordPhraseTranslations
                            .FirstOrDefault(x => x.WordPhraseID == wordPhrase.WordPhraseID);
                        var trText = ar[1].Trim().FirstCharToUpper().LastCharToDot();

                        if (wordPhraseTranslation == null)
                        {
                            wordPhraseTranslation = new WordPhraseTranslation()
                            {
                                LanguageID = spaAppSetting.NativeLanguage.LanguageID,
                                Text = trText,
                                WordPhraseID = wordPhrase.WordPhraseID
                            };

                            ent.WordPhraseTranslations.Add(wordPhraseTranslation);
                        }
                        else
                        {
                            wordPhraseTranslation.Text = trText;
                            ent.WordPhraseTranslations.Update(wordPhraseTranslation);
                        }

                        ent.SaveChanges();
                    }
                }
            }
        }

        //public async Task AddListFromTextWithPhrasesV2(string sourceText, SpaAppSetting spaAppSetting, bool withMix = false)
        //{
        //    var allWords = _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID).ToList();
        //    var arr = sourceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        //    Word currWord = null;
        //    var phOrder = 1000;
        //    foreach (var line in arr)
        //    {
        //        if (line.StartsWith("++"))
        //        {
        //            phOrder = 1000;
        //            // word
        //            var ar = line.Split((new string[] { "++", "///"}) as string[], StringSplitOptions.RemoveEmptyEntries);
        //            var wordText = ar[0].Trim();
        //            currWord = allWords.FirstOrDefault(x => x.TextClear.ToLower() == wordText.ToLower() || x.Text.ToLower() == wordText.ToLower());
        //            if (currWord == null)
        //            {
        //                continue;
        //            }

        //            // translation

        //            if (ar.Length == 1)
        //            {
        //                continue;
        //            }

        //            var wTranslation = ar[1].Trim();
        //            if (string.IsNullOrEmpty(wTranslation))
        //            {
        //                continue;
        //            }

        //            var wordTranslation = _context.WordTranslations
        //                .FirstOrDefault(x=>
        //                    x.LanguageID == spaAppSetting.NativeLanguage.LanguageID 
        //                    && x.WordID == currWord.WordID
        //                    );

        //            if (wordTranslation == null)
        //            {
        //                wordTranslation = new WordTranslation()
        //                {
        //                    WordID = currWord.WordID,
        //                    LanguageID = spaAppSetting.NativeLanguage.LanguageID,
        //                    Text = wTranslation,
        //                };

        //                _context.Add(wordTranslation);
        //            }
        //            else
        //            {
        //                wordTranslation.Text = wTranslation;
        //                _context.Update(wordTranslation);
        //            }

        //            _context.SaveChanges();
        //        }
        //        else if (line.Trim().StartsWith("##"))
        //        {
        //            // phrase
        //            var ar = line.Trim().Split((new string[] { "##", "///" }) as string[], StringSplitOptions.RemoveEmptyEntries);
        //            var text = ar[0].Trim().FirstCharToUpper().LastCharToDot();

        //            WordPhrase wordPhrase;
        //            using (var ent = new ddPoliglotDbContext(_configuration))
        //            {
        //                wordPhrase = ent.WordPhrases.AsNoTracking().FirstOrDefault(x => x.Text == text);
        //                if (wordPhrase == null)
        //                {
        //                    wordPhrase = new WordPhrase()
        //                    {
        //                        LanguageID = (int)spaAppSetting.LearnLanguage.LanguageID,
        //                        Text = text
        //                    };
        //                }
        //                else
        //                {
        //                    wordPhrase.Text = text;
        //                }

        //                wordPhrase.HashCode = wordPhrase.GetHashCode();

        //                if (wordPhrase.WordPhraseID > 0)
        //                {
        //                    ent.Update(wordPhrase);
        //                }
        //                else
        //                {
        //                    ent.WordPhrases.Add(wordPhrase);
        //                }

        //                var wordPhraseWord = ent.WordPhraseWords.AsNoTracking()
        //                    .FirstOrDefault(x =>
        //                        x.WordID == currWord.WordID
        //                        && x.WordPhraseID == wordPhrase.WordPhraseID
        //                    );

        //                if (wordPhraseWord == null)
        //                {
        //                    wordPhraseWord = new WordPhraseWord()
        //                    {
        //                        WordID = currWord.WordID,
        //                        WordPhrase = wordPhrase,
        //                        PhraseOrder = phOrder--
        //                    };

        //                    wordPhrase.WordPhraseWords.Add(wordPhraseWord);
        //                    currWord.WordPhraseWords.Add(wordPhraseWord);
        //                }
        //                else
        //                {
        //                    wordPhraseWord.PhraseOrder = phOrder--;
        //                    ent.Update(wordPhraseWord);
        //                }

        //                ent.SaveChanges();

        //                if (ar.Length == 1)
        //                {
        //                    continue;
        //                }

        //                var wordPhraseTranslation = ent.WordPhraseTranslations
        //                    .FirstOrDefault(x => x.WordPhraseID == wordPhrase.WordPhraseID);

        //                if (wordPhraseTranslation == null)
        //                {
        //                    wordPhraseTranslation = new WordPhraseTranslation()
        //                    {
        //                        LanguageID = spaAppSetting.NativeLanguage.LanguageID,
        //                        Text = ar[1].Trim(),
        //                        WordPhraseID = wordPhrase.WordPhraseID
        //                    };

        //                    ent.WordPhraseTranslations.Add(wordPhraseTranslation);
        //                }
        //                else
        //                {
        //                    wordPhraseTranslation.Text = ar[1].Trim();
        //                    ent.WordPhraseTranslations.Update(wordPhraseTranslation);
        //                }

        //                ent.SaveChanges();

        //            }

        //        }
        //    }
        //}

        public async Task AddListFromTextWithPhrasesV4(string sourceText, SpaAppSetting spaAppSetting, bool withMix = false)
        {
            var wordsNewVersion = JsonConvert.DeserializeObject<List<Word>>(sourceText);
            var wordsOldVersion = GetWordsWithFirstPhAndTr(spaAppSetting, 20, 6000, true);
            var sb = new StringBuilder(); 

            if (wordsNewVersion.Count > 400)
            {
                // all database map, all other should be marks as deleted
                foreach (var word in wordsOldVersion)
                {
                    word.Deleted = true;
                }
            }

            foreach (var newWord in wordsNewVersion)
            {
                var oldWord = wordsOldVersion.FirstOrDefault(x => x.TextClear == newWord.TextClear);
                if (oldWord == null)
                {
                    sb.AppendLine($"!!!!!! add Word {newWord.Text} with phrases");
                    // create new word with all relations
                    oldWord = (Word)newWord.Clone();
                    oldWord.WordID = 0;
                    oldWord.WordPhraseWords = new List<WordPhraseWord>();
                    oldWord.WordPhraseSelected = null;
                    oldWord.WordTranslation = null;
                    oldWord.WordUser = null;
                    _context.Words.Add(oldWord);
                    _context.SaveChanges();
                    _context.Entry(oldWord).State = EntityState.Detached;

                    var wordTranslation = (WordTranslation)newWord.WordTranslation.Clone();
                    wordTranslation.WordTranslationID = 0;
                    wordTranslation.WordID = oldWord.WordID;
                    _context.WordTranslations.Add(wordTranslation);
                    _context.SaveChanges();
                    _context.Entry(wordTranslation).State = EntityState.Detached;

                    foreach (var newWordPhrase in newWord.WordPhraseSelected)
                    {
                        var oldWordPhrase = (WordPhrase)newWordPhrase.Clone();
                        oldWordPhrase.WordPhraseID = 0;
                        _context.WordPhrases.Add(oldWordPhrase);
                        _context.SaveChanges();
                        _context.Entry(oldWordPhrase).State = EntityState.Detached;

                        var wordPhraseWord = new WordPhraseWord
                        {
                            WordID = oldWord.WordID,
                            WordPhraseID = oldWordPhrase.WordPhraseID,
                            PhraseOrder = oldWordPhrase.PhraseOrderInCurrentWord,
                        };

                        _context.WordPhraseWords.Add(wordPhraseWord);
                        _context.SaveChanges();
                        _context.Entry(wordPhraseWord).State = EntityState.Detached;

                        var wordPhraseTranslation = (WordPhraseTranslation)newWordPhrase.WordPhraseTranslation.Clone();
                        wordPhraseTranslation.WordPhraseTranslationID = 0;
                        wordPhraseTranslation.WordPhraseID = oldWordPhrase.WordPhraseID;

                        _context.WordPhraseTranslations.Add(wordPhraseTranslation);
                        _context.SaveChanges();
                        _context.Entry(wordPhraseTranslation).State = EntityState.Detached;
                    }
                }
                else
                {
                    // check changes between old and new text and translations

                    var diff = oldWord.Compare(newWord);
                    Word word = null;
                    // overwrite old version with new one
                    word = (Word)newWord.Clone();
                    word.WordID = oldWord.WordID;
                    word.WordPhraseWords = new List<WordPhraseWord>();
                    word.WordPhraseSelected = null;
                    word.WordTranslation = null;
                    word.WordUser = null;

                    if (!String.IsNullOrEmpty(diff))
                    {
                        sb.AppendLine($"+++++++ word {word.TextClear} changed: {diff}");
                    }

                    _context.Words.Update(word);
                    _context.SaveChanges();
                    _context.Entry(word).State = EntityState.Detached;

                    if (wordsNewVersion.Count > 400)
                    {
                        oldWord.Deleted = false;                    
                    }


                    if (oldWord.WordTranslation == null)
                    {
                        var wordTranslation = (WordTranslation)newWord.WordTranslation.Clone();
                        wordTranslation.WordTranslationID = 0;
                        wordTranslation.WordID = oldWord.WordID;

                        sb.AppendLine($"+++++++ add WordTranslations {wordTranslation.Text}");

                        _context.WordTranslations.Add(wordTranslation);
                        _context.SaveChanges();
                        _context.Entry(wordTranslation).State = EntityState.Detached;
                    }
                    else
                    {
                        diff = oldWord.WordTranslation.Compare(newWord.WordTranslation);

                        var wordTranslation = (WordTranslation)newWord.WordTranslation.Clone();
                        wordTranslation.WordTranslationID = oldWord.WordTranslation.WordTranslationID;
                        wordTranslation.WordID = oldWord.WordID;

                        if (!String.IsNullOrEmpty(diff))
                        {
                            sb.AppendLine($"+++++++ WordTranslations {oldWord.WordTranslation.Text} changed: {diff}");
                        }

                        _context.WordTranslations.Update(wordTranslation);
                        _context.SaveChanges();
                        _context.Entry(wordTranslation).State = EntityState.Detached;
                    }


                    foreach (var newWordPhrase in newWord.WordPhraseSelected)
                    {
                        var oldWordPhrase = oldWord.WordPhraseSelected.FirstOrDefault(x=>x.Text == newWordPhrase.Text);
                        if (oldWordPhrase == null)
                        {
                            sb.AppendLine($"!!!!!! add WordPhrase {newWordPhrase.Text} with translation to word {word.Text}");
                            // new phrase
                            oldWordPhrase = (WordPhrase)newWordPhrase.Clone();
                            oldWordPhrase.WordPhraseID = 0;
                            _context.WordPhrases.Add(oldWordPhrase);
                            _context.SaveChanges();
                            _context.Entry(oldWordPhrase).State = EntityState.Detached;

                            var wordPhraseWord = new WordPhraseWord
                            {
                                WordID = oldWord.WordID,
                                WordPhraseID = oldWordPhrase.WordPhraseID,
                                PhraseOrder = oldWordPhrase.PhraseOrderInCurrentWord,
                            };

                            _context.WordPhraseWords.Add(wordPhraseWord);
                            _context.SaveChanges();
                            _context.Entry(wordPhraseWord).State = EntityState.Detached;

                            var wordPhraseTranslation = (WordPhraseTranslation)newWordPhrase.WordPhraseTranslation.Clone();
                            wordPhraseTranslation.WordPhraseTranslationID = 0;
                            wordPhraseTranslation.WordPhraseID = oldWordPhrase.WordPhraseID;

                            _context.WordPhraseTranslations.Add(wordPhraseTranslation);
                            _context.SaveChanges();
                            _context.Entry(wordPhraseTranslation).State = EntityState.Detached;
                        }
                        else
                        {
                            // overvrite old phrase

                            diff = oldWordPhrase.Compare(newWordPhrase);

                            var wordPhrase = (WordPhrase)newWordPhrase.Clone();
                            wordPhrase.WordPhraseID = oldWordPhrase.WordPhraseID;

                            if (!String.IsNullOrEmpty(diff))
                            {
                                sb.AppendLine($"+++++++ WordPhrase {newWordPhrase.Text} changed: {diff}");
                            }

                            _context.WordPhrases.Update(wordPhrase);
                            _context.SaveChanges();
                            _context.Entry(wordPhrase).State = EntityState.Detached;

                            // override order
                            if (oldWordPhrase.WordPhraseWordInCurrentWord.PhraseOrder != wordPhrase.PhraseOrderInCurrentWord)
                            {
                                sb.AppendLine($"+++++++ WordPhrase {newWordPhrase.Text} changed order: {oldWordPhrase.WordPhraseWordInCurrentWord.PhraseOrder} -> {wordPhrase.PhraseOrderInCurrentWord}");
                            }

                            var wordPhraseWord = oldWordPhrase.WordPhraseWordInCurrentWord;
                            wordPhraseWord.Word = null;
                            wordPhraseWord.WordPhrase = null;
                            wordPhraseWord.PhraseOrder = wordPhrase.PhraseOrderInCurrentWord;
                            _context.WordPhraseWords.Update(wordPhraseWord);
                            _context.SaveChanges();
                            _context.Entry(wordPhraseWord).State = EntityState.Detached;

                            // override translation

                            if (oldWordPhrase.WordPhraseTranslation == null)
                            {
                                var wordPhraseTranslation = (WordPhraseTranslation)newWordPhrase.WordPhraseTranslation.Clone();
                                wordPhraseTranslation.WordPhraseTranslationID = 0;
                                wordPhraseTranslation.WordPhraseID = oldWordPhrase.WordPhraseID;
                                sb.AppendLine($"+++++++ add WordPhraseTranslation {wordPhraseTranslation.Text}");
 
                                _context.WordPhraseTranslations.Add(wordPhraseTranslation);
                                _context.SaveChanges();
                                _context.Entry(wordPhraseTranslation).State = EntityState.Detached;
                            }
                            else
                            {
                                diff = oldWordPhrase.WordPhraseTranslation.Compare(newWordPhrase.WordPhraseTranslation);
                                var wordPhraseTranslation = (WordPhraseTranslation)newWordPhrase.WordPhraseTranslation.Clone();
                                wordPhraseTranslation.WordPhraseTranslationID = oldWordPhrase.WordPhraseTranslation.WordPhraseTranslationID;
                                wordPhraseTranslation.WordPhraseID = oldWordPhrase.WordPhraseTranslation.WordPhraseID;
                                if (!String.IsNullOrEmpty(diff))
                                {
                                    sb.AppendLine($"+++++++ WordPhraseTranslation {newWordPhrase.WordPhraseTranslation.Text} changed: {diff}");
                                }

                                _context.WordPhraseTranslations.Update(wordPhraseTranslation);
                                _context.SaveChanges();
                                _context.Entry(wordPhraseTranslation).State = EntityState.Detached;
                            }
                        }
                    }
                }
            }

            if (wordsNewVersion.Count > 400)
            {
                foreach (var word in wordsNewVersion)
                {
                    if (word.Deleted)
                    {
                        sb.AppendLine($"------ word {word.TextClear} mark as deleted");
                        _context.Database.ExecuteSqlRaw("update words set deleted = 1 where wordID = {0}", word.WordID);
                        _context.SaveChanges();
                    }
                }
            }

            var result = sb.ToString();
            if (!string.IsNullOrEmpty(result))
            {
                var versions = _context.DictionaryVersions.Where(x =>
                    x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                    && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                    ).Select(x=>x.Value).ToList();

                var lastVersion = versions.Count == 0
                    ? 7
                    : versions.Max(x=>x);
                
                var dictionaryVersion = new DictionaryVersion()
                {
                    LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                    NativeLanguageID = spaAppSetting.NativeLanguage.LanguageID,
                    Value = lastVersion + 1,
                    Description = result
                };

                _context.DictionaryVersions.Add(dictionaryVersion);
                _context.SaveChanges();
            }
        }


        public async Task<List<Word>> GetListForRepetition(SpaAppSetting spaAppSetting, Guid userId, int parentID, string baseName, int maxWordsForRepetition = 35)
        {
            var wordRequiredRepetitionInLessonsQty = 3;
            var result = new List<Word>();

            #region get elder lessons with same base name

            var oldArticlesByParams = _context.ArticleByParams.AsNoTracking()
                .Where(x => x.UserID == userId
                && x.LearnLanguageID == spaAppSetting.LearnLanguage.LanguageID
                && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                ).ToList();

            oldArticlesByParams = oldArticlesByParams
                .Where(x => x.Name.StartsWith(baseName) 
                && (parentID == 0 || x.ArticleByParamID < parentID)
                )
                .OrderByDescending(x => x.ArticleByParamID)
                .ToList();

            #endregion get elder lessons with same base name

            #region get words for repetition from 3 latest lessons

            var latestArticlesByParams = oldArticlesByParams
                .OrderByDescending(x => x.ArticleByParamID)
                .Take(wordRequiredRepetitionInLessonsQty)
                .ToList();

            var latestArticlesByParamsIndex = 0;
            foreach (var articlesByParams in latestArticlesByParams)
            {
                var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(articlesByParams.DataJson);
                foreach (var wordSelected in articleByParamData.SelectedWords)
                {
                    result.Add(wordSelected.Word);

                    // get phrase
                    var index = latestArticlesByParamsIndex < wordSelected.PhraseWordsSelected.Count()
                        ? latestArticlesByParamsIndex
                        : 0;

                    wordSelected.Word.WordPhraseSelected = new List<WordPhrase>();
                    wordSelected.Word.WordPhraseSelected.Add(wordSelected.PhraseWordsSelected[index]);
                }

                latestArticlesByParamsIndex++;
            }

            #endregion get words for repetition from 3 latest lessons

            #region get current lesson number by order from previous lessons

            var query1 = _context.WordUsers.AsNoTracking()
                    .Where(x =>
                        x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                        && x.UserID == userId
                        && (parentID == 0 || x.LastRepeatInArticleByParamID < parentID)
                        );

            var curLessonNumber = 1;
            if (query1.Count() > 0)
            {
                curLessonNumber = query1.Max(x => x.LastRepeatInLessonNum) + 1;
            }

            #endregion get current lesson number by order from previous lessons

            #region get words for repetition by grade and history

            if (parentID > 0)
            {
                // clear all already related words
                _context.Database.ExecuteSqlRaw($"update WordUsers set LastRepeatWordPhraseId = 0," +
                    $" LastRepeatInArticleByParamID = 0, LastRepeatInLessonNum = 0" +
                    $" where LastRepeatInArticleByParamID = {parentID}");
            }

            // get words candidates to local list
            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID && !x.Deleted)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID } into grouping1
                from wtj in grouping1.DefaultIfEmpty()
                join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                from wuj in grouping2.DefaultIfEmpty()
                select new Word()
                {
                    LanguageID = w.LanguageID,
                    Pronunciation = w.Pronunciation,
                    Rate = w.Rate,
                    Text = w.Text,
                    HashCode = w.HashCode,
                    HashCodeSpeed1 = w.HashCodeSpeed1,
                    HashCodeSpeed2 = w.HashCodeSpeed2,
                    SpeachDuration = w.SpeachDuration,
                    SpeachDurationSpeed1 = w.SpeachDurationSpeed1,
                    SpeachDurationSpeed2 = w.SpeachDurationSpeed2,
                    TextSpeechFileName = w.TextSpeechFileName,
                    TextSpeechFileNameSpeed1 = w.TextSpeechFileNameSpeed1,
                    TextSpeechFileNameSpeed2 = w.TextSpeechFileNameSpeed2,
                    WordID = w.WordID,
                    WordTranslation = wtj,
                    WordUser = wuj,
                };

            var words = query
                .Where(x => x.WordUser != null && x.WordUser.Grade > 0 && x.WordUser.Grade < 5)
                .OrderByDescending(x => (5 - x.WordUser.Grade) * (curLessonNumber - x.WordUser.LastRepeatInLessonNum))
                .ThenBy(x => x.Rate)
                .Take(maxWordsForRepetition + result.Count())
                .ToList();

            FillWordListWithPhrases(words, spaAppSetting, false);

            foreach (var word in words)
            {
                if (!result.Any(x => x.WordID == word.WordID))
                {
                    if (result.Count() < maxWordsForRepetition)
                    {
                        result.Add(word);

                        // get phrase for this word
                        word.WordPhraseSelected = word.WordPhraseSelected
                            .Where(x=>x.PhraseOrderInCurrentWord > 0)
                            .Where(x=>x.WordPhraseTranslation != null)
                            .ToList();

                        var phrase = GetPhraseForRepetition(word);
                        word.WordPhraseSelected = new List<WordPhrase>();
                        if (phrase != null)
                        {
                            word.WordPhraseSelected.Add(phrase);
                        }
                    }
                }
            }

            #endregion get words for repetition by grade and history

            return result;
        }

        public async Task<List<Word>> GetListForRepetition1(SpaAppSetting spaAppSetting, Guid userId, int lastLessonNumber, int maxWordsForRepetition = 35)
        {
            var wordRequiredRepetitionInLessonsQty = 3;

            #region get words for repetition from 3 latest lessons

            var result = await (from ul in _context.UserLessons.AsNoTracking()
                        join ulw in _context.UserLessonWords.AsNoTracking() on ul.UserLessonID equals ulw.UserLessonID
                        join w in _context.Words on ulw.WordID equals w.WordID
                        join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID } into grouping1
                        from wtj in grouping1.DefaultIfEmpty()
                        join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                        from wuj in grouping2.DefaultIfEmpty()
                        where 
                            ul.UserID == userId 
                            && ul.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                            && wuj.Grade < 5
                            && wuj.Grade > 0
                            && ulw.wordType == 0
                            && ul.Num > (lastLessonNumber - wordRequiredRepetitionInLessonsQty)
                                orderby ul.Num descending

                        select new Word()
                        {
                            LanguageID = w.LanguageID,
                            Pronunciation = w.Pronunciation,
                            Rate = w.Rate,
                            Text = w.Text,
                            HashCode = w.HashCode,
                            HashCodeSpeed1 = w.HashCodeSpeed1,
                            HashCodeSpeed2 = w.HashCodeSpeed2,
                            SpeachDuration = w.SpeachDuration,
                            SpeachDurationSpeed1 = w.SpeachDurationSpeed1,
                            SpeachDurationSpeed2 = w.SpeachDurationSpeed2,
                            TextSpeechFileName = w.TextSpeechFileName,
                            TextSpeechFileNameSpeed1 = w.TextSpeechFileNameSpeed1,
                            TextSpeechFileNameSpeed2 = w.TextSpeechFileNameSpeed2,
                            WordID = w.WordID,
                            WordTranslation = wtj,
                            WordUser = wuj,
                            RateTmp3 = ul.Num,
                        }).ToListAsync();

            #endregion get words for repetition from 3 latest lessons

            #region get words for repetition by grade and history

            var ids = result.Select(x => x.WordID).ToList();

            // get words candidates to local list
            var query = 
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID && !x.Deleted)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID } into grouping1
                from wtj in grouping1.DefaultIfEmpty()
                join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                from wuj in grouping2.DefaultIfEmpty()
                select new Word()
                {
                    LanguageID = w.LanguageID,
                    Pronunciation = w.Pronunciation,
                    Rate = w.Rate,
                    Text = w.Text,
                    HashCode = w.HashCode,
                    HashCodeSpeed1 = w.HashCodeSpeed1,
                    HashCodeSpeed2 = w.HashCodeSpeed2,
                    SpeachDuration = w.SpeachDuration,
                    SpeachDurationSpeed1 = w.SpeachDurationSpeed1,
                    SpeachDurationSpeed2 = w.SpeachDurationSpeed2,
                    TextSpeechFileName = w.TextSpeechFileName,
                    TextSpeechFileNameSpeed1 = w.TextSpeechFileNameSpeed1,
                    TextSpeechFileNameSpeed2 = w.TextSpeechFileNameSpeed2,
                    WordID = w.WordID,
                    WordTranslation = wtj,
                    WordUser = wuj,
                };

            var words = await query
                .Where(x=> !ids.Contains(x.WordUser.WordID))
                .Where(x => x.WordUser != null && x.WordUser.Grade > 0 && x.WordUser.Grade < 5)
                .OrderByDescending(x => (5 - x.WordUser.Grade) * (lastLessonNumber - x.WordUser.LastRepeatInLessonNum))
                .ThenBy(x => x.Rate)
                .Take(maxWordsForRepetition)
                .ToListAsync();

            result.AddRange(words);

            FillWordListWithPhrases(result, spaAppSetting, true);

            foreach (var word in result)
            {
                var phrase = GetPhraseForRepetition(word);
                word.WordPhraseSelected = new List<WordPhrase>();
                if (phrase != null)
                {
                    word.WordPhraseSelected.Add(phrase);
                }
            }

            #endregion get words for repetition by grade and history

            return result;
        }

        public WordPhrase GetPhraseForRepetition(Word word)
        {
            if (word.WordPhraseSelected.Count() == 0)
            {
                return null;
            }

            if (word.WordUser.LastRepeatWordPhraseId == 0)
            {
                return word.WordPhraseSelected[0];
            }

            var index = 0;
            foreach (var phrase in word.WordPhraseSelected)
            {
                if (phrase.WordPhraseID == word.WordUser.LastRepeatWordPhraseId)
                {
                    break;
                }

                index++;
            }

            if (index >= (word.WordPhraseSelected.Count() - 1))
            {
                index = 0;
            }
            else
            {
                index++;
            }

            return word.WordPhraseSelected[index];
        }

        public List<Word> GetWordsWithFirstPhAndTr(SpaAppSetting spaAppSetting, int maxPhrases, int maxorder, bool allNeed)
        {
            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == 1 && !x.Deleted)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, LanguageID = 2 } equals new { wt.WordID, wt.LanguageID } into grouping1
                from wtj in grouping1.DefaultIfEmpty()
                where !w.Deleted && w.Rate <= maxorder
                orderby w.Rate
                select new
                {
                    Word = w,
                    WordTranslation = wtj,
                };

            var wordsWithTr = query.ToList();
            foreach (var wordWithTr in wordsWithTr)
            {
                wordWithTr.Word.WordTranslation = wordWithTr.WordTranslation;
            }

            var words = wordsWithTr.Select(x => x.Word).ToList();
            var wordIDs = words.Select(x => x.WordID).ToList();

            var query2 =
                from wpw in _context.WordPhraseWords.AsNoTracking()
                join wp in _context.WordPhrases.AsNoTracking() on wpw.WordPhraseID equals wp.WordPhraseID
                join wpt in _context.WordPhraseTranslations on new
                {
                    wp.WordPhraseID,
                    LanguageID = 2
                }
                equals new
                {
                    wpt.WordPhraseID,
                    wpt.LanguageID
                }
                into grouping1
                from wptj in grouping1.DefaultIfEmpty()
                where wordIDs.Contains(wpw.WordID) && wpw.PhraseOrder >= 1100
                select new
                {
                    WordPhrase = wp,
                    WordPhraseTranslation = wptj,
                    WordPhraseWord = wpw
                };

            var phsWithTr = query2.ToList();
            foreach (var phWithTr in phsWithTr)
            {
                phWithTr.WordPhrase.WordPhraseTranslation = phWithTr.WordPhraseTranslation;
                phWithTr.WordPhrase.CurrentWordID = phWithTr.WordPhraseWord.WordID;
                phWithTr.WordPhrase.PhraseOrderInCurrentWord = phWithTr.WordPhraseWord.PhraseOrder;
                phWithTr.WordPhrase.WordPhraseWordInCurrentWord = phWithTr.WordPhraseWord;
            }

            var wordPhrases = phsWithTr.Select(x => x.WordPhrase).ToList();

            var resultWords = new List<Word>();   
            foreach (var word in words)
            {
                word.WordPhraseSelected = wordPhrases.Where(x => x.CurrentWordID == word.WordID).OrderByDescending(x => x.PhraseOrderInCurrentWord).Take(maxPhrases).ToList();
                if (word.WordPhraseSelected.Count >= maxPhrases || allNeed)
                {
                    resultWords.Add(word);
                }
                else
                {
                    int a = 1;
                }
            }

            return resultWords;
        }
    }
}
