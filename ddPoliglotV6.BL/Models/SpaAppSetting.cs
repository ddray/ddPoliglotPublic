using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ddPoliglotV6.BL.Models
{
    public class SpaAppSetting
    {
        public SpaAppSetting(string value)
        {
        }

        public SpaAppSetting(Language nativeLanguage, Language learnLanguage)
        {
            NativeLanguage = nativeLanguage;
            LearnLanguage = learnLanguage;
        }

        public SpaAppSetting(IHeaderDictionary headers, Data.ddPoliglotDbContext _context)
        {
            var headerKey = "spa-app-settings";
            if (headers.ContainsKey(headerKey))
            {
                StringValues value;
                if (headers.TryGetValue(headerKey, out value))
                {
                    var ar = value.ToString().Split(';');
                    if (ar.Length == 2)
                    {
                        var nativeLanguageID = Convert.ToInt32(ar[0]);
                        var learnLanguageID = Convert.ToInt32(ar[1]);
                        var languages = _context.Languages.AsNoTracking().ToList();
                        this.LearnLanguage = languages.FirstOrDefault(x=>x.LanguageID == learnLanguageID);
                        this.NativeLanguage = languages.FirstOrDefault(x => x.LanguageID == nativeLanguageID);
                    }
                }
            }
        }

        public Language NativeLanguage { get; set; }
        public Language LearnLanguage { get; set; }
    }
}
