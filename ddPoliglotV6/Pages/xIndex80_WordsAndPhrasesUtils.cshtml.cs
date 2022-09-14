using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ddPoliglotV6.Pages
{
    [Authorize]
    public class xIndex80_WordsAndPhrasesUtilsModel : PageModel
    {
        private readonly ILogger<xIndex80_WordsAndPhrasesUtilsModel> _logger;

        public xIndex80_WordsAndPhrasesUtilsModel(ILogger<xIndex80_WordsAndPhrasesUtilsModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }


        public class xx
        {
            public string name { get; set; }
            public int a { get; set; }
            public int b { get; set; }
        }

        //public async Task<IActionResult> IndexAsync()
        //{
        //    int wordId = 3;


        //    #region update words, set first letter in uppercase
        //    using (var transaction = _context.Database.BeginTransaction())
        //    {
        //        var words = _context.Words.AsNoTracking().ToList();
        //        foreach (var word in words)
        //        {
        //            word.Text = word.Text.First().ToString().ToUpper() + word.Text.Substring(1);
        //        }

        //        _context.BulkUpdate(words);
        //        transaction.Commit();
        //    }

        //    #endregion update words, set first letter in uppercase

        //    #region copy from local db to cloud

        //    //using (var transaction = _context.Database.BeginTransaction())
        //    //{
        //    //    using (var dbLocal = new ddPoliglotLocalDbContext(_configuration))
        //    //    {
        //    //        var words = dbLocal.WordTranslations.AsNoTracking().ToList();
        //    //        //_context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT WordPhrases ON;");
        //    //        //_context.AddRange(words);
        //    //        //_context.SaveChanges();

        //    //        _context.BulkInsert(words);

        //    //        //_context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT WordPhrases OFF;");
        //    //        transaction.Commit();
        //    //    }
        //    //    // _context.BulkUpdate(updatedWp);
        //    //    //transaction.Commit();
        //    //}

        //    #endregion copy from local db to cloud



        //    #region delete and relink dublicates phrases

        //    //var query = _context.WordPhrases.AsNoTracking()
        //    //    .GroupBy(x => x.HashCode)
        //    //    .Where(g => g.Count() > 1)
        //    //    .OrderByDescending(x => x.Count())
        //    //    .Select(g => new { g.Key, count = g.Count()});

        //    //var lst = query.ToList();

        //    //var updatedWpw = new List<WordPhraseWord>();
        //    //var updatedWp = new List<WordPhrase>();
        //    //foreach (var key in lst)
        //    //{
        //    //    var wps = _context.WordPhrases.AsNoTracking().Where(x => x.HashCode == key.Key).OrderBy(y => y.WordPhraseID).ToList();
        //    //    var wpFirst = wps[0];
        //    //    for (int i = 1; i < wps.Count(); i++)
        //    //    {
        //    //        var wp = wps[i];
        //    //        wp.HashCode = 0;
        //    //        updatedWp.Add(wp);
        //    //        var wpws = _context.WordPhraseWords.Where(x => x.WordPhraseID == wp.WordPhraseID).ToList();
        //    //        foreach (var wpw in wpws)
        //    //        {
        //    //            wpw.WordPhraseID = wpFirst.WordPhraseID;
        //    //            updatedWpw.Add(wpw);
        //    //        }
        //    //    }

        //    //    if (updatedWp.Count() > 100)
        //    //    {
        //    //        using (var transaction = _context.Database.BeginTransaction())
        //    //        {
        //    //            _context.BulkUpdate(updatedWp);
        //    //            transaction.Commit();
        //    //        }

        //    //        updatedWp.Clear();
        //    //    }

        //    //    if (updatedWpw.Count() > 100)
        //    //    {
        //    //        using (var transaction = _context.Database.BeginTransaction())
        //    //        {
        //    //            _context.BulkUpdate(updatedWpw);
        //    //            transaction.Commit();
        //    //        }

        //    //        updatedWpw.Clear();
        //    //    }
        //    //}

        //    //var query = _context.WordPhraseWords.AsNoTracking()
        //    //    .GroupBy(x => new { x.WordID, x.WordPhraseID })
        //    //    .Where(g => g.Count() > 1)
        //    //    .OrderByDescending(x => x.Count())
        //    //    .Select(g => new { g.Key, count = g.Count() });

        //    //var lst = query.ToList();

        //    //var deletedWpw = new List<WordPhraseWord>();
        //    //foreach (var key in lst)
        //    //{
        //    //    var wpws = _context.WordPhraseWords.AsNoTracking()
        //    //        .Where(x => x.WordID == key.Key.WordID && x.WordPhraseID == key.Key.WordPhraseID)
        //    //        .OrderBy(y => y.WordPhraseID)
        //    //        .ToList();

        //    //    for (int i = 1; i < wpws.Count(); i++)
        //    //    {
        //    //        deletedWpw.Add(wpws[i]);
        //    //    }

        //    //    if (deletedWpw.Count() > 1000)
        //    //    {
        //    //        using (var transaction = _context.Database.BeginTransaction())
        //    //        {
        //    //            _context.BulkDelete(deletedWpw);
        //    //            transaction.Commit();
        //    //        }

        //    //        deletedWpw.Clear();
        //    //    }
        //    //}

        //    //if (deletedWpw.Count() > 0)
        //    //{
        //    //    using (var transaction = _context.Database.BeginTransaction())
        //    //    {
        //    //        _context.BulkDelete(deletedWpw);
        //    //        transaction.Commit();
        //    //    }

        //    //    deletedWpw.Clear();
        //    //}


        //    int ss = 1;
        //    #endregion  delete and relink dublicates phrases

        //    #region check and reassign Hash code

        //    //var updatedItems = new List<WordPhrase>();
        //    //var ids = new int[] {73519, 61425, 34851};
        //    //var wordPhrases = _context.WordPhrases.AsNoTracking()
        //    //    //.Where(x => x.SourceType == 0)
        //    //    //.Where(x=> ids.Contains(x.WordPhraseID))
        //    //    .OrderByDescending(x => x.WordPhraseID).ToList();

        //    //var cnt = 0;
        //    //foreach (var wordPhrase in wordPhrases)
        //    //{
        //    //    wordPhrase.HashCode = wordPhrase.GetPhraseHash();
        //    //    updatedItems.Add(wordPhrase);
        //    //    cnt++;
        //    //    if (cnt > 1000)
        //    //    {
        //    //        using (var transaction = _context.Database.BeginTransaction())
        //    //        {
        //    //            _context.BulkUpdate(updatedItems);
        //    //            transaction.Commit();
        //    //        }

        //    //        updatedItems.Clear();
        //    //        cnt = 0;
        //    //    }

        //    //    Debug.Print(cnt.ToString());
        //    //}

        //    //if (updatedItems.Count > 0)
        //    //{
        //    //    using (var transaction = _context.Database.BeginTransaction())
        //    //    {
        //    //        _context.BulkUpdate(updatedItems);
        //    //        transaction.Commit();
        //    //    }

        //    //    updatedItems.Clear();
        //    //}

        //    //int ss = 1;
        //    #endregion  check and reassign Hash code

        //    #region xx
        //    //var query1 =
        //    //    from w in _context.Words
        //    //    join wt in _context.WordTranslations on new { w.WordID, LanguageID = (int)Languages.ru } equals new { wt.WordID, wt.LanguageID } into grouping1
        //    //        from wtj in grouping1.DefaultIfEmpty()
        //    //    where w.WordID == wordId
        //    //    select new Word() 
        //    //    {
        //    //        HashCode = w.HashCode,
        //    //        LanguageID = w.LanguageID,
        //    //        Pronunciation = w.Pronunciation,
        //    //        Rate = w.Rate,
        //    //        Text = w.Text,
        //    //        WordTranslation = wtj,
        //    //        WordID = w.WordID,
        //    //    };

        //    //var list = query1.ToList();
        //    //int c = 1;


        //    //var query1 = from wpw in _context.WordPhraseWords
        //    //             join wp in _context.WordPhrases on wpw.WordPhraseID equals wp.WordPhraseID
        //    //             where wpw.WordID == wordId
        //    //             select wp;
        //    //var words = _context.Words.Take(10).ToList();
        //    //var wordIds = words.Select(x=>x.WordID).ToList();
        //    ////var wordPhraseIds = new List<int>
        //    ////    words.Select(x=)

        //    //// IQueryable<Word> query = _context.Words;
        //    //var query1 = from wpw in _context.WordPhraseWords
        //    //             join wp in _context.WordPhrases on wpw.WordPhraseID equals wp.WordPhraseID
        //    //             where wordIds.Contains(wpw.WordID)
        //    //             orderby wpw.WordID
        //    //             select new { wp, wpw.WordID }
        //    //             ;
        //    //var list = query1.ToList();

        //    //list.ForEach(x => 
        //    //{

        //    //});
        //    //foreach (var item in list)
        //    //{

        //    //}

        //    //foreach (var item in list)
        //    //{

        //    //}
        //    //var ll = list.GroupBy(x => x.w).Select(y => new { y.Key, lst = y.Select(z => z.wp) });
        //    //var words = _context.Words.Include(x=>x.WordPhraseWords).Take(10).ToList();

        //    //IQueryable<WordPhraseWord> query = _context.WordPhraseWords.Include(x=>x.Word).Include(y=>y.WordPhrase);

        //    #endregion xx

        //    #region reassign wordPhrases for words

        //    //var words = _context.Words.Include(x => x.WordPhraseWords).ToList();
        //    //var wordPhrases = _context.WordPhrases.Where(x => x.SourceType == 0).OrderByDescending(x => x.WordPhraseID).ToList();
        //    //var cnt = 0;
        //    //var newWordPhraseWords = new List<WordPhraseWord>();
        //    //foreach (var wordPhrase in wordPhrases)
        //    //{
        //    //    var ar = wordPhrase.Text.Split(
        //    //        new char[] { ',', '.', '!', '?', ';', ':', ' ' },
        //    //        StringSplitOptions.RemoveEmptyEntries
        //    //        );

        //    //    foreach (var subword in ar)
        //    //    {
        //    //        if (subword.Length > 3)
        //    //        {
        //    //            var mutchWord = words.Where(x => (subword.ToLower().StartsWith(x.Text.ToLower()))).FirstOrDefault();
        //    //            if (mutchWord != null)
        //    //            {
        //    //                if (!mutchWord.WordPhraseWords.Any(x => x.WordPhraseID == wordPhrase.WordPhraseID)
        //    //                    && mutchWord.WordPhraseWords.Count() < 1000)
        //    //                {
        //    //                    var newWordPhraseWord = new WordPhraseWord()
        //    //                    {
        //    //                        Word = mutchWord,
        //    //                        WordID = mutchWord.WordID,
        //    //                        WordPhrase = wordPhrase,
        //    //                        WordPhraseID = wordPhrase.WordPhraseID,
        //    //                    };

        //    //                    mutchWord.WordPhraseWords.Add(newWordPhraseWord);
        //    //                    //wordPhrase.WordPhraseWords.Add(newWordPhraseWord);

        //    //                    //_context.Entry(newWordPhraseWord).State = EntityState.Added;
        //    //                    //_context.SaveChanges();
        //    //                    newWordPhraseWords.Add(newWordPhraseWord);
        //    //                }
        //    //            }
        //    //        }
        //    //    }

        //    //if (newWordPhraseWords.Count > 1000)
        //    //{
        //    //    using (var transaction = _context.Database.BeginTransaction())
        //    //    {
        //    //        _context.BulkInsert(newWordPhraseWords);
        //    //        transaction.Commit();
        //    //    }

        //    //    newWordPhraseWords.Clear();
        //    //}

        //    //    cnt++;
        //    //    Debug.Print($"cnt: {cnt}");
        //    //}

        //    //if (newWordPhraseWords.Count() > 0)
        //    //{
        //    //    using (var transaction = _context.Database.BeginTransaction())
        //    //    {
        //    //        _context.BulkInsert(newWordPhraseWords);
        //    //        transaction.Commit();
        //    //    }

        //    //    newWordPhraseWords.Clear();
        //    //}

        //    #endregion reassign wordPhrases for words

        //    #region get words from text file
        //    //int counter = 0;
        //    //string line;
        //    //char[] delimiterChars = { ' ', '\t' };

        //    //System.IO.StreamReader file =
        //    //    new System.IO.StreamReader(@"e:\5000_EnglishWords.txt");
        //    //while ((line = file.ReadLine()) != null)
        //    //{
        //    //    var ar = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

        //    //    if (ar.Length > 2 && counter > 0)
        //    //    {
        //    //        var text = ar[1];
        //    //        int rate = Convert.ToInt32(ar[0]);
        //    //        Word word = new Word
        //    //        {
        //    //            LanguageID = (int)Languages.en,
        //    //            Text = text,
        //    //            Rate = rate
        //    //        };

        //    //        _context.Words.Add(word);
        //    //        // Debug.WriteLine($"{ar[0]} - {ar[1]} = {line}");
        //    //    }
        //    //    else 
        //    //    {
        //    //        // Debug.WriteLine($" = {line}");
        //    //    }

        //    //    counter++;
        //    //}

        //    //_context.SaveChanges();

        //    //file.Close();
        //    //Debug.WriteLine(counter.ToString());

        //    //System.IO.StreamReader file =
        //    //    new System.IO.StreamReader(@"e:\json.txt");

        //    //var json = file.ReadToEnd();

        //    #endregion get words from text file

        //    #region get word phrases from WordsApiv1
        //    //var skip = 1000;
        //    //var take = 2000;

        //    //var words = _context.Words.Where(y=>y.Rate > 2999).OrderBy(x => x.Rate).ToList();
        //    //foreach (var word in words)
        //    //{
        //    //    var json = await GetData(word.Text);
        //    //    var jObject = JObject.Parse(json);

        //    //    var pronunciation = (((Newtonsoft.Json.Linq.JProperty)jObject["pronunciation"]?.FirstOrDefault())?.Value ?? "").ToString();
        //    //    word.Pronunciation = pronunciation;
        //    //    _context.Update(word);
        //    //    _context.SaveChanges();

        //    //    var examples = new List<string>();

        //    //    if (jObject["results"] != null)
        //    //    {
        //    //        var results = jObject["results"].ToList();
        //    //        foreach (var item in results)
        //    //        {
        //    //            if (item["examples"] != null && item["examples"].HasValues)
        //    //            {
        //    //                foreach (string exmpl in item["examples"].ToList())
        //    //                {
        //    //                    WordPhrase wordPhrase = new WordPhrase() { LanguageID = (int)Languages.en, Text = exmpl };
        //    //                    wordPhrase.HashCode = wordPhrase.GetHashCode();
        //    //                    WordPhraseWord wordPhraseWord = new WordPhraseWord() { WordID = word.WordID, WordPhrase = wordPhrase };
        //    //                    wordPhrase.WordPhraseWords.Add(wordPhraseWord);
        //    //                    word.WordPhraseWords.Add(wordPhraseWord);
        //    //                    _context.WordPhrases.Add(wordPhrase);
        //    //                    _context.SaveChanges();
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        int a = 0;
        //    //    }
        //    //}

        //    #endregion get word phrases from WordsApiv1

        //    #region get word phrases from Cambridge

        //    //var words = _context.Words.Where(y => y.Rate >= 2749).OrderBy(x => x.Rate).ToList();
        //    //foreach (var word in words)
        //    //{
        //    //    if (word.Text.Length > 3)
        //    //    {
        //    //        var result = await GetDataCambridge(word.Text);
        //    //        Debug.Print($"{word.Rate} {word.Text} : {result.Count()}");

        //    //        foreach (var item in result)
        //    //        {
        //    //            WordPhrase wordPhrase = new WordPhrase() { LanguageID = (int)Languages.en, Text = item };
        //    //            wordPhrase.HashCode = wordPhrase.GetHashCode();
        //    //            WordPhraseWord wordPhraseWord = new WordPhraseWord() { WordID = word.WordID, WordPhrase = wordPhrase };
        //    //            wordPhrase.WordPhraseWords.Add(wordPhraseWord);
        //    //            word.WordPhraseWords.Add(wordPhraseWord);
        //    //            _context.WordPhrases.Add(wordPhrase);
        //    //            _context.SaveChanges();

        //    //        }
        //    //    }
        //    //}

        //    #endregion get word phrases from Cambridge

        //    #region get word translation from Yandex

        //    //var wordTranslations = new List<WordTranslation>();

        //    //var words = _context.Words.Where(y => y.Rate >= 0).OrderBy(x => x.Rate).ToList();
        //    //foreach (var word in words)
        //    //{
        //    //    if (word.Text.Length > 0)
        //    //    {
        //    //        var results = await GetDataYandex(word.Text);
        //    //        var result = string.Join(",", results.Phrases);

        //    //        if (result.Length > 240)
        //    //        {
        //    //            result = result.Substring(0, 230);
        //    //        }

        //    //        Debug.Print($"{word.Rate} {word.Text} : {result}");

        //    //        if (string.IsNullOrEmpty(word.Pronunciation) && !string.IsNullOrEmpty(results.Pronunciation))
        //    //        {
        //    //            word.Pronunciation = results.Pronunciation;
        //    //            _context.Update(word);
        //    //            _context.SaveChanges();
        //    //        }

        //    //        if (!string.IsNullOrEmpty(result))
        //    //        {
        //    //            var wordTranslation = new WordTranslation()
        //    //            {
        //    //                WordID = word.WordID,
        //    //                LanguageID = (int)Languages.ru,
        //    //                Text = result,
        //    //            };

        //    //            wordTranslations.Add(wordTranslation);
        //    //        }
        //    //    }

        //    //    if (wordTranslations.Count() > 100)
        //    //    {
        //    //        using (var transaction = _context.Database.BeginTransaction())
        //    //        {
        //    //            _context.BulkInsert(wordTranslations);
        //    //            transaction.Commit();
        //    //        }

        //    //        wordTranslations.Clear();
        //    //    }

        //    //}

        //    //if (wordTranslations.Count() > 0)
        //    //{
        //    //    using (var transaction = _context.Database.BeginTransaction())
        //    //    {
        //    //        _context.BulkInsert(wordTranslations);
        //    //        transaction.Commit();
        //    //    }

        //    //    wordTranslations.Clear();
        //    //}



        //    #endregion get word translation from Yandex

        //    return View();
        //}

        //public class WordData
        //{
        //    public string Pronunciation;
        //    public List<string> Phrases;
        //}
        //public class RetryHandler : DelegatingHandler
        //{
        //    // Strongly consider limiting the number of retries - "retry forever" is
        //    // probably not the most user friendly way you could respond to "the
        //    // network cable got pulled out."
        //    private const int MaxRetries = 5;

        //    public RetryHandler(HttpMessageHandler innerHandler)
        //        : base(innerHandler)
        //    { }

        //    protected override async Task<HttpResponseMessage> SendAsync(
        //        HttpRequestMessage request,
        //        CancellationToken cancellationToken)
        //    {
        //        HttpResponseMessage response = null;
        //        for (int i = 0; i < MaxRetries; i++)
        //        {
        //            try
        //            {
        //                response = await base.SendAsync(request, cancellationToken);
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    return response;
        //                }
        //                else
        //                {
        //                    Debug.Print($"DELAY START _________________________------------------");
        //                    await Task.Delay(10000);
        //                    Debug.Print($"DELAY END  *********************************************");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Debug.Print($"DELAY2 START _________________________------------------");
        //                await Task.Delay(10000);
        //                Debug.Print($"DELAY2 END  *********************************************");
        //            }
        //        }

        //        return response;
        //    }
        //}

        //public class BusinessLogic
        //{
        //    public void FetchSomeThingsSynchronously()
        //    {
        //        // ...

        //        // Consider abstracting this construction to a factory or IoC container
        //        using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
        //        {
        //            myResult = client.PostAsync(yourUri, yourHttpContent).Result;
        //        }

        //        // ...
        //    }
        //}


        //static async Task<WordData> GetDataYandex(string text)
        //{
        //    var result = new WordData();
        //    result.Phrases = new List<string>();
        //    string baseUrl = $"https://dictionary.yandex.net/api/v1/dicservice.json/lookup?key=dict.1.1.20200229T180936Z.03fec99b9db00f4d.e9db0c431c31e7cf1feae316826aefbe0183ce72&lang=en-ru&text={text}";
        //    string data = "";

        //    using (HttpClient client = new HttpClient(new RetryHandler(new HttpClientHandler())))
        //    {
        //        using (HttpResponseMessage res = await client.GetAsync(baseUrl))
        //        {
        //            using (HttpContent content = res.Content)
        //            {
        //                data = await content.ReadAsStringAsync();
        //                await Task.Delay(100);
        //            }
        //        }
        //    }

        //    //System.IO.StreamReader file =
        //    //    new System.IO.StreamReader(@"e:\yandexDict_example2.txt");
        //    //data = file.ReadToEnd();

        //    var jObject = JObject.Parse(data);
        //    if (jObject["def"] != null)
        //    {
        //        var defs = jObject["def"].ToList();
        //        if (defs.Count > 0)
        //        {
        //            if (defs[0]?["ts"] != null)
        //            {
        //                result.Pronunciation = ((Newtonsoft.Json.Linq.JValue)defs[0]["ts"]).Value.ToString();
        //            }
        //        }

        //        foreach (var item in defs)
        //        {
        //            if (item["tr"] != null)
        //            {
        //                var trs = item["tr"].ToList();
        //                foreach (var tr in trs)
        //                {
        //                    var value = ((Newtonsoft.Json.Linq.JValue)tr["text"]).Value.ToString();
        //                    if (!string.IsNullOrEmpty(value))
        //                    {
        //                        result.Phrases.Add(value);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return result;
        //}

        //static async Task<string> GetDataWordsApiv1(string text)
        //{
        //    string baseUrl = $"https://wordsapiv1.p.rapidapi.com/words/{text}";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Accept", "application/json");
        //        client.DefaultRequestHeaders.Add("x-rapidapi-host", "wordsapiv1.p.rapidapi.com");
        //        client.DefaultRequestHeaders.Add("x-rapidapi-key", "c0a1d38cd1msh207937123719598p1a9fdbjsn6e21ec862d47");
        //        using (HttpResponseMessage res = await client.GetAsync(baseUrl))
        //        {
        //            using (HttpContent content = res.Content)
        //            {
        //                string data = await content.ReadAsStringAsync();
        //                return data;
        //            }
        //        }
        //    }
        //}

        //static async Task<List<string>> GetDataCambridge(string text)
        //{
        //    var result = new List<string>();
        //    string baseUrl = $"https://dictionary.cambridge.org/dictionary/english/{text}";
        //    string data = "";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        using (HttpResponseMessage res = await client.GetAsync(baseUrl))
        //        {
        //            using (HttpContent content = res.Content)
        //            {
        //                data = await content.ReadAsStringAsync();
        //            }
        //        }
        //    }

        //    //System.IO.StreamReader file =
        //    //    new System.IO.StreamReader(@"e:\camb_example2.txt");
        //    //data = file.ReadToEnd();

        //    var ar = data.Split(new string[]
        //    {
        //        "<span class=\"eg deg\">",
        //        //"examp\"><span class=\"eg deg\">",
        //        //"examp\"> <span class=\"eg deg\">",
        //        "examp hax\">",
        //        //"examp",
        //    }, StringSplitOptions.RemoveEmptyEntries);

        //    for (int i = 1; i < ar.Length; i++)
        //    {
        //        var str = Regex.Replace(ar[i], "<.*?>", String.Empty);
        //        //string pattern = @"(\()|(\))|(\d+)|(.)|(!)|(?)|(/)|(\[)|(\])";

        //        var ar2 = Regex.Split(str, @"(?<=[\.!\?])\s+");
        //        if (ar2.Length > 1 && ar2[0].Length < 200 && ar2[0].Length > 10)
        //        {
        //            result.Add(ar2[0]);
        //        }
        //    }

        //    //var ar = data.Split("examp\"><span class=\"eg deg\">", StringSplitOptions.RemoveEmptyEntries);

        //    return result;
        //}


    }
}
