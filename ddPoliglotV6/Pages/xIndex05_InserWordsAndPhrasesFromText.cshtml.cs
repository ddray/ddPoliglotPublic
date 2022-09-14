using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ddPoliglotV6.BL.Enums;
using ddPoliglotV6.BL.Extentions;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using static ddPoliglotV6.BL.Managers.TranslationManager;

namespace ddPoliglotV6.Pages
{
    public class xIndex05_InserWordsAndPhrasesFromTextModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private WordManager _wordManager;


        public xIndex05_InserWordsAndPhrasesFromTextModel(
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

        private readonly List<int> SchemaOfWordRepInWordPhrases = new List<int>() { 8, 16, 32, 64, 128, 256, 512, 1024 };

        public List<ResView> Results { get; set; } = new List<ResView>();

        public async Task OnGet()
        {
            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == 1)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, LanguageID = 2 } equals new { wt.WordID, wt.LanguageID }
                select new Word()
                {
                    LanguageID = w.LanguageID,
                    Pronunciation = w.Pronunciation,
                    Rate = w.Rate,
                    Text = w.Text,
                    OxfordLevel = w.OxfordLevel,
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
                    WordTranslation = wt,
                };

 
            var words = await query
                .Where(x=>x.Rate > 700)
                .OrderBy(x => x.Rate)
                .Take(300).ToListAsync();

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

            var wordPhrases = query2.ToList();

            foreach (var word in words)
            {
                word.WordPhraseSelected = wordPhrases.Where(x => x.CurrentWordID == word.WordID).OrderBy(x => x.WordPhraseID).ToList();
            }

            Results = new List<ResView>();
            foreach (var word in words)
            {
                Results.Add(new ResView()
                {
                    Word = word,
                    Candidstes = word.WordPhraseSelected
                        .Select(x => new Candidat() 
                            { 
                                WordPhrase = x, 
                                Words = new List<Word>(), 
                                wordsBeforeCnt = 0 
                            }
                        )
                        .ToList()

                });  
            }
        }
        private List<Candidat> GetCandidates(Word currentWord, List<int> usedPhraseId)
        {
            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                // get all phrases which used this word
                var phrases = (from wpw in ent.WordPhraseWords.AsNoTracking()
                               join wp in ent.WordPhrases.AsNoTracking() on wpw.WordPhraseID equals wp.WordPhraseID
                               join wpw1 in ent.WordPhraseWords.AsNoTracking() on wp.WordPhraseID equals wpw1.WordPhraseID
                               join w in ent.Words.AsNoTracking() on wpw1.WordID equals w.WordID
                               where wpw.WordID == currentWord.WordID // && wpw.PhraseOrder > 1000
                               select new
                               {
                                   ph = wp,
                                   wr = w
                               }).ToList();


                var candidates1 = new List<Candidat>();
                var currentCandifat = new Candidat()
                {
                    Words = new List<Word>()
                };

                foreach (var phrase in phrases.Where(x => !usedPhraseId.Contains(x.ph.WordPhraseID)).OrderBy(x => x.ph.WordPhraseID))
                {
                    if (currentCandifat.WordPhrase?.WordPhraseID != phrase.ph.WordPhraseID)
                    {
                        if (currentCandifat.WordPhrase?.WordPhraseID != null)
                        {
                            currentCandifat.Calc(currentWord);
                            candidates1.Add(currentCandifat);
                        }

                        currentCandifat = new Candidat()
                        {
                            WordPhrase = phrase.ph,
                            Words = new List<Word>()
                        };
                    }

                    if (phrase.wr.WordID != currentWord.WordID && phrase.wr.RateTmp2 == 0)
                    {
                        currentCandifat.Words.Add(phrase.wr);
                    }
                }

                if (!candidates1.Any(x=>x.WordPhrase.WordPhraseID == currentCandifat.WordPhrase.WordPhraseID))
                {
                    if (currentCandifat.WordPhrase != null)
                    {
                        candidates1.Add(currentCandifat);
                    }
                }

                return candidates1;
            }

        }

        private void GetWordsForRepetition(Word currWord, List<Word> words, List<Word> wordsLineForRepeat)
        {
            var currWordIndex = words.FindIndex(x => x.WordID == currWord.WordID);

            // get words from schema
            foreach (var val in this.SchemaOfWordRepInWordPhrases)
            {
                var wordIndex = val * 5;
                var index = currWordIndex - wordIndex;
                if (index >= 0)
                {
                    if (!wordsLineForRepeat.Any(x => x.WordID == words[index].WordID))
                    {
                        wordsLineForRepeat.Add(words[index]);
                    }
                }
            }
        }

        //var candidates1 = new List<Candidat>();
        //foreach (var phrase in phrases)
        //{
        //    var allWords = GetAllWordTextsFromPhrase(phrase.WordPhraseID);
        //    unlistedWords.AddRange(allWords.Where(x=>x.WordID == 0).ToList());
        //    candidates1.Add(new Candidat()
        //    { 
        //         WordPhrase = phrase,
        //         Words = allWords
        //    });
        //}

        public class ResView
        {
            public Word Word { get; set; }
            public List<Candidat> Candidstes { get; set; }
        }

        public class Candidat
        { 
            public WordPhrase WordPhrase { get; set; }
            public List<Word> Words { get; set; }

            public int wordsBeforeCnt { get; set; }
            public int wordsAfterCnt
            {
                get
                {
                    return this.Words.Count() - this.wordsBeforeCnt;
                }
            }

            public int Height { 
                get {
                    return this.wordsBeforeCnt - this.wordsAfterCnt; 
                } 
            }

            public int Clear
            {
                get
                {
                    return this.wordsAfterCnt == 0 ? 0 : 1;
                }
            }

            public int Before
            {
                get
                {
                    return this.wordsBeforeCnt == 0 ? 1 : 0;
                }
            }

            public void Calc(Word currWord)
            {
                this.wordsBeforeCnt = this.Words.Count(x => x.Rate <= currWord.Rate);
            }
        }

        private List<Word> GetAllWordTextsFromPhrase(int wordPhraseID)
        {
            char[] delimiterChars = { ' ', '\t', '\'', '.', ',', '!', '?', '\"' };

            var result = new List<Word>();
            var sql = @" SELECT
			                    kw.display_term as SpeechFileName, case when sw.stopword is null then Cast(0 as bigint) else Cast(1 as bigint) end as HashCode, CAST( (ROW_NUMBER() OVER(ORDER BY kw.display_term ASC)) as int) as SpeechFileID, 0 as Duration, 0 as [Version]
                            FROM sys.dm_fts_index_keywords_by_document(db_id('ddPoliglotV6'), object_id('WordPhrases')) as kw
                                left join sys.fulltext_system_stopwords as sw on sw.stopword = kw.display_term and sw.language_id = 1033
                           where
                               document_id = " + wordPhraseID.ToString() + @"
                               and kw.display_term <> 'END OF FILE'";


            var texts = _context.SpeechFiles.FromSqlRaw(sql).AsNoTracking().ToList();
            foreach (var item in texts)
            {
                var phraseWordText = char.ToUpper(item.SpeechFileName[0]) + item.SpeechFileName.Substring(1);
                var ar = phraseWordText.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                var text = ar[0];
                var isNoise = item.HashCode == 1;
                try
                {
                    var connectedWords = isNoise
                        ? _context.Words.Where(x => x.Text == text).ToList()
                        : _context.Words
                        .FromSqlRaw($"Select * from [Words] where CONTAINS([Text], 'FORMSOF(INFLECTIONAL,{phraseWordText})')")
                        .ToList();
                    
                    if (connectedWords.Count == 0)
                    {
                        // this word is not in list
                        result.Add(new Word() { Text = text });
                    }
                    else
                    {
                        result.Add(connectedWords.OrderBy(x=>x.Rate).First());
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print($"!!!!!!!!!!!!!! error word {phraseWordText}, {ex.Message.ToString()}");
                }
            }

            return result;
        }

        private void BondWordsToPhrases()
        {
            #region set phrases to word
            var wordPhrasesWords = _context.WordPhraseWords.AsNoTracking().ToList();

            var wordPhrases = _context.WordPhrases.AsNoTracking().OrderBy(x => x.WordPhraseID).ToList();
            var words = _context.Words.AsNoTracking().ToList();
            var cnt = 0;

            foreach (var wordPhrase in wordPhrases)
            {
                Debug.Print($"phrase {wordPhrase.WordPhraseID}: {wordPhrase.Text}");
                // get list of words from this phrase
                var sql = @" SELECT
			                    kw.display_term as SpeechFileName, case when sw.stopword is null then Cast(0 as bigint) else Cast(1 as bigint) end as HashCode, CAST( (ROW_NUMBER() OVER(ORDER BY kw.display_term ASC)) as int) as SpeechFileID, 0 as Duration, 0 as [Version]
                            FROM sys.dm_fts_index_keywords_by_document(db_id('ddPoliglotV6'), object_id('WordPhrases')) as kw
                                left join sys.fulltext_system_stopwords as sw on sw.stopword = kw.display_term and sw.language_id = 1033
                           where
                               document_id = " + wordPhrase.WordPhraseID.ToString() + @"
                               and kw.display_term <> 'END OF FILE'";


                var result = _context.SpeechFiles.FromSqlRaw(sql).AsNoTracking().ToList();
                foreach (var item in result)
                {
                    var phraseWordText = char.ToUpper(item.SpeechFileName[0]) + item.SpeechFileName.Substring(1);
                    var isNoise = item.HashCode == 1;
                    if (phraseWordText.Contains("'"))
                    {
                        continue;
                    }

                    try
                    {
                        var connectedWords = isNoise
                            ? words.Where(x => x.Text == phraseWordText).ToList()
                            : _context.Words
                            .FromSqlRaw($"Select * from [Words] where CONTAINS([Text], 'FORMSOF(INFLECTIONAL,{phraseWordText})')")
                            .ToList();

                        foreach (var word in connectedWords)
                        {
                            var wordPhraseWord = wordPhrasesWords.Where(x => x.WordID == word.WordID && x.WordPhraseID == wordPhrase.WordPhraseID).FirstOrDefault();
                            if (wordPhraseWord == null)
                            {
                                wordPhraseWord = new WordPhraseWord()
                                {
                                    WordID = word.WordID,
                                    WordPhraseID = wordPhrase.WordPhraseID
                                };

                                _context.WordPhraseWords.Add(wordPhraseWord);
                                if (++cnt > 5000)
                                {
                                    _context.SaveChanges();
                                    cnt = 0;
                                }

                                wordPhrasesWords.Add(wordPhraseWord);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print($"!!!!!!!!!!!!!! error word {phraseWordText}, {ex.Message.ToString()}");
                    }
                }
            }

            #endregion spred new words from oxford by level
        }

        private void MarkNoiseWords()
        {
            #region MarkNoiseWords
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\NoiceWords.txt");

            string line;
            char[] delimiterChars = { ' ', '\t' };
            var tmpRate = 1;
            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var ar = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                line = ar[0].Trim().TrimEnd('\n').TrimEnd('\r');
                var text = char.ToUpper(line[0]) + line.Substring(1);

                var sameWord = _context.Words.FirstOrDefault(x=>x.Text == text);
                if (sameWord != null)
                {
                    sameWord.RateTmp2 = 1;
                    _context.Words.Update(sameWord);
                    tmpRate++;
                }
            }

            _context.SaveChanges();

            file.Close();

            #endregion MarkNoiseWords
        }

        private async void GetFromOxf()
        {
            #region set phrases to word

            var words = _context.Words.AsNoTracking().OrderBy(x => x.Rate).ToList();
            foreach (var word in words)
            {
                var result = await GetDataFromOxford(word.Text);
                Debug.Print($"{word.Rate} {word.Text} : {result.Count()}");

                foreach (var item in result)
                {
                    WordPhrase wordPhrase = new WordPhrase() { LanguageID = (int)Languages.en, Text = item };
                    wordPhrase.HashCode = wordPhrase.GetHashCode();
                    wordPhrase.SourceType = 10;
                    // WordPhraseWord wordPhraseWord = new WordPhraseWord() { WordID = word.WordID, WordPhrase = wordPhrase };
                    // wordPhrase.WordPhraseWords.Add(wordPhraseWord);
                    // word.WordPhraseWords.Add(wordPhraseWord);
                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        ent.WordPhrases.Add(wordPhrase);
                        ent.SaveChanges();
                    }
                }
            }
            #endregion spred new words from oxford by level

        }

        static async Task<List<string>> GetDataFromOxford(string text)
        {
            var result = new List<string>();
            string baseUrl = $"https://www.oxfordlearnersdictionaries.com/definition/english/{text.ToLower()}_1?q={text.ToLower()}";
            string data = "";
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                {
                    using (HttpContent content = res.Content)
                    {
                        data = await content.ReadAsStringAsync();
                    }
                }
            }

            //System.IO.StreamReader file =
            //    new System.IO.StreamReader(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\oxfordpage-example.txt");
            //data = file.ReadToEnd();

            var ar = data.Split(new string[]
            {
                "<span class=\"x\">",
            }, StringSplitOptions.RemoveEmptyEntries);

            //foreach (var item in ar)
            //{
            //    var ar2 = 
            //}

            for (int i = 1; i < ar.Length; i++)
            {
                var ss = ar[i];
                var ar3 = ss.Split(new string[]
                {
                "</span></li>",
                }, StringSplitOptions.RemoveEmptyEntries);
                var ss2 = ar3[0];
                var str = Regex.Replace(ss2, "<.*?>", String.Empty);
                result.Add(str);
            }

            return result;
        }

        public void ResetRate()
        {
            #region reset rate

            var words = _context.Words.AsNoTracking().OrderBy(x => x.Rate).ToList();
            var rate = 1;
            foreach (var word in words)
            {
                word.Rate = rate++;
                _context.Update(word);
            }

            _context.SaveChanges();
            #endregion spred new words from oxford by level
        }

        public void GetWordsFromOxfSetRateByLevel()
        {
            #region spred new words from oxford by level

            var words = _context.Words.AsNoTracking().OrderBy(x => x.Rate).ToList();
            // get words whish did not find analog in main list, so set rate by rnd and level            
            var wordsToSpread = _context.Words.AsNoTracking().Where(x => x.Rate > 7000 && x.OxfordLevel > 0).ToList();

            foreach (var word in wordsToSpread)
            {
                Random r = new Random();

                var rndRateByLevel = word.OxfordLevel == 1
                    ? r.Next(150, 1000)
                    : word.OxfordLevel == 2
                        ? r.Next(1001, 2000)
                        : word.OxfordLevel == 3
                            ? r.Next(2001, 3000)
                            : word.OxfordLevel == 4
                                ? r.Next(3001, 4000)
                                : word.OxfordLevel == 5
                                    ? r.Next(4001, 6000)
                                    : -1;
                if (rndRateByLevel < 0)
                {
                    var a2 = 1;
                }

                word.Rate = rndRateByLevel;
                _context.Update(word);
            }

            _context.SaveChanges();
            #endregion spred new words from oxford by level

        }

        public void GetWordsFromOldTextFileAndPutToDatabase()
        {
            #region get words from text file
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\old5000.txt");

            string line;
            char[] delimiterChars = { ' ', '\t' };

            var rate = 15000;
            var tmpRate = 1;
            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var ar = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                line = ar[0].Trim().TrimEnd('\n').TrimEnd('\r');
                var text = char.ToUpper(line[0]) + line.Substring(1);

                if (string.IsNullOrWhiteSpace(text) || text.Length < 4)
                {
                    continue;
                }

                var sameWords = _context.Words.FromSqlRaw($"Select * from [Words] where CONTAINS([Text],'FORMSOF(INFLECTIONAL,{text.Replace("'", "''")})')").ToList();
                if (sameWords.Count == 0)
                {
                    Word word = new Word
                    {
                        LanguageID = (int)Languages.en,
                        Text = text,
                        Rate = rate,
                        RateTmp = tmpRate
                    };

                    rate++;

                    _context.Words.Add(word);
                    _context.SaveChanges();
                }

                tmpRate++;
            }


            file.Close();
            #endregion get words from text file
        }

        public void GetWordsFromOxfordTextFileAndPutToDatabase()
        {
            #region get words from text file
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\oxf5000.txt");

            string line;
            char[] delimiterChars = { ' ', '\t' };

            var rate = 11000;
            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var ar = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                line = ar[0].Trim().TrimEnd('\n').TrimEnd('\r');
                var text = char.ToUpper(line[0]) + line.Substring(1);
                var level = 0;
                foreach (var item in ar)
                {
                    if (item.Contains("A1"))
                    {
                        level = 1;
                    }
                    else if (item.Contains("A2"))
                    {
                        level = 2;
                    }
                    else if (item.Contains("B1"))
                    {
                        level = 3;
                    }
                    else if (item.Contains("B2"))
                    {
                        level = 4;
                    }
                    else if (item.Contains("C1"))
                    {
                        level = 5;
                    }

                    if (level != 0)
                    {
                        break;
                    }
                }

                if (level == 0)
                {
                    var a = 1; // throw new Exception();
                }

                var sameWords = _context.Words.FromSqlRaw($"Select * from [Words] where CONTAINS([Text],'FORMSOF(INFLECTIONAL,{text})')").ToList();
                if (sameWords.Count == 0)
                {
                    Word word = new Word
                    {
                        LanguageID = (int)Languages.en,
                        Text = text,
                        Rate = rate,
                        OxfordLevel = level
                    };

                    rate++;

                    _context.Words.Add(word);
                    _context.SaveChanges();
                }
                else if (sameWords.Count > 1)
                {
                    var word = sameWords.OrderBy(x => x.Rate).Take(1).FirstOrDefault();
                    word.OxfordLevel = level;
                    _context.Update(word);
                    _context.SaveChanges();
                }
                else if (sameWords.Count == 1)
                {
                    var word = sameWords[0];
                    word.OxfordLevel = level;
                    _context.Update(word);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception();
                }

            }


            file.Close();
            #endregion get words from text file

        }
        public void GetWordsFromTextFileAndPutToDatabase()
        {
            #region get words from text file
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\tmp.txt");
            var rate = 1;
            var words = new List<Word>();
            string line = "";

            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                line = line.Trim().TrimEnd('\n').TrimEnd('\r');
                var text = char.ToUpper(line[0]) + line.Substring(1);

                if (!words.Any(x => x.Text == text))
                {
                    Word word = new Word
                    {
                        LanguageID = (int)Languages.en,
                        Text = text,
                        Rate = rate
                    };

                    words.Add(word);
                }

                rate++;
            }

            foreach (var word in words)
            {
                _context.Words.Add(word);
            }

            _context.SaveChanges();

            file.Close();
            #endregion get words from text file

        }

        static async Task<WordData> GetDataYandex(string text)
        {
            var result = new WordData();
            result.Phrases = new List<string>();
            string baseUrl = $"https://dictionary.yandex.net/api/v1/dicservice.json/lookup?key=dict.1.1.20210623T124856Z.2addca5b314f3486.b493b0eb21dface02c5f4b37e8de29b8eb0023cb&lang=en-ru&text={text}";
            string data = "";

            using (HttpClient client = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                {
                    using (HttpContent content = res.Content)
                    {
                        data = await content.ReadAsStringAsync();
                        await Task.Delay(100);
                    }
                }
            }

            //System.IO.StreamReader file =
            //    new System.IO.StreamReader(@"e:\yandexDict_example2.txt");
            //data = file.ReadToEnd();

            var jObject = JObject.Parse(data);
            if (jObject["def"] != null)
            {
                var defs = jObject["def"].ToList();
                if (defs.Count > 0)
                {
                    if (defs[0]?["ts"] != null)
                    {
                        result.Pronunciation = ((Newtonsoft.Json.Linq.JValue)defs[0]["ts"]).Value.ToString();
                    }
                }

                foreach (var item in defs)
                {
                    if (item["tr"] != null)
                    {
                        var trs = item["tr"].ToList();
                        foreach (var tr in trs)
                        {
                            var value = ((Newtonsoft.Json.Linq.JValue)tr["text"]).Value.ToString();
                            if (!string.IsNullOrEmpty(value))
                            {
                                result.Phrases.Add(value);
                            }
                        }
                    }
                }
            }

            return result;
        }


        private void nepravGlag()
        {
            #region nepraveln glagol

            var words = _context.Words.AsNoTracking().ToList();

            System.IO.StreamReader file =
                new System.IO.StreamReader(@"D:\000_Work_7\ddPoliglot\ddPoliglotV6\ClientAppV6\src\txt\nepravGlagols.txt");

            string line;
            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var arr = line.Replace("\t", "").Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                if (arr.Length != 7)
                {
                    int a = 0;
                }

                var w1 = arr[0].Replace("[", "").Replace("]", "").Trim().FirstCharToUpper();
                var p1 = arr[1].Replace("[", "").Replace("]", "").Trim();
                var w2 = arr[2].Replace("[", "").Replace("]", "").Trim();
                var p2 = arr[3].Replace("[", "").Replace("]", "").Trim();
                var w3 = arr[4].Replace("[", "").Replace("]", "").Trim();
                var p3 = arr[5].Replace("[", "").Replace("]", "").Trim();
                var tr = arr[6].Replace("[", "").Replace("]", "").Trim().FirstCharToUpper();


                var text = $"{w1}, {w2}, {w3}";
                var pron = $"{p1}, {p2}, {p3}";

                var word = words.FirstOrDefault(x => x.Text.ToLower() == w1.ToLower());
                if (word != null)
                {
                    word.Text = text;
                    word.Pronunciation = pron;
                    _context.Update(word);
                }
            }

            _context.SaveChanges();

            file.Close();
            #endregion get words from text file

        }


    }
}
