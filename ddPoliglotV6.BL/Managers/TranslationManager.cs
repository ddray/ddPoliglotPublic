using ddPoliglotV6.BL.Extentions;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using ddPoliglotV6.BL.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Google.Cloud.Translation.V2;
using Grpc.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ddPoliglotV6.BL.Helpers;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ddPoliglotV6.BL.Managers
{
    public class TranslationManager
    {
        //private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public TranslationManager(ddPoliglotDbContext context,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //_context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public string Translate(TranslateArg args)
        {
            var result = "";
            var credential = GoogleCredential.FromFile(FilesIOHelper.TranslationKeyFileName(_hostingEnvironment.WebRootPath));
            TranslationClient clientTR = TranslationClient.Create(credential);

            var responseTR = clientTR.TranslateText(args.SourceText, args.TargetLanguage, args.SourceLanguage, TranslationModel.ServiceDefault);
            result = responseTR.TranslatedText;

            return result;
        }

        public List<string> TranslateArray(TranslateArrayArg args)
        {
            var result = new List<string>();
            var credential = GoogleCredential.FromFile(FilesIOHelper.TranslationKeyFileName(_hostingEnvironment.WebRootPath));
            TranslationClient clientTR = TranslationClient.Create(credential);
            foreach (string itemStr in args.Source)
            {
                var responseTR = clientTR.TranslateText(itemStr, args.TargetLanguage, args.SourceLanguage, TranslationModel.ServiceDefault);
                result.Add(responseTR.TranslatedText);
            }

            return result;
        }

        public static async Task<WordData> GetDataYandex(string text)
        {
            var result = new WordData();
            result.Phrases = new List<string>();
            string baseUrl = $"https://dictionary.yandex.net/api/v1/dicservice.json/lookup?key=dict.1.1.20200229T180936Z.03fec99b9db00f4d.e9db0c431c31e7cf1feae316826aefbe0183ce72&lang=en-ru&text={text}";
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

        public class WordData
        {
            public string Pronunciation;
            public List<string> Phrases;
        }

        public class RetryHandler : DelegatingHandler
        {
            // Strongly consider limiting the number of retries - "retry forever" is
            // probably not the most user friendly way you could respond to "the
            // network cable got pulled out."
            private const int MaxRetries = 5;

            public RetryHandler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            { }

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                HttpResponseMessage response = null;
                for (int i = 0; i < MaxRetries; i++)
                {
                    try
                    {
                        response = await base.SendAsync(request, cancellationToken);
                        if (response.IsSuccessStatusCode)
                        {
                            return response;
                        }
                        else
                        {
                            Debug.Print($"DELAY START _________________________------------------");
                            await Task.Delay(10000);
                            Debug.Print($"DELAY END  *********************************************");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print($"DELAY2 START _________________________------------------");
                        await Task.Delay(10000);
                        Debug.Print($"DELAY2 END  *********************************************");
                    }
                }

                return response;
            }
        }
    }
}
