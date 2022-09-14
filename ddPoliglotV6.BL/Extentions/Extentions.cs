using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data.Models;
using System;
using System.Linq;

namespace ddPoliglotV6.BL.Extentions
{
    public static class Extentions
    {
        public static int GetPhraseHash(this ArticlePhrase articlePhrase, string text = null)
        {
             return Managers.HashManager.GetHashFromTextAndVoice(
                articlePhrase.Text,
                articlePhrase.ArticleActor.Language,
                articlePhrase.ArticleActor.VoiceName,
                articlePhrase.ArticleActor.VoicePitch,
                articlePhrase.ArticleActor.VoiceSpeed
                );
        }

        public static int GetTrPhraseHash(this ArticlePhrase articlePhrase, string text = null)
        {
            return Managers.HashManager.GetHashFromTextAndVoice(
               articlePhrase.TrText,
               articlePhrase.TrArticleActor.Language,
               articlePhrase.TrArticleActor.VoiceName,
               articlePhrase.TrArticleActor.VoicePitch,
               articlePhrase.TrArticleActor.VoiceSpeed
               );
        }

        //public static int GetPhraseHash(this WordTranslation wordTranslation, string text = null)
        //{
        //    if (string.IsNullOrEmpty(wordTranslation.Text))
        //    {
        //        return 0;
        //    }

        //    var str = (wordTranslation.Text.Trim()
        //        + wordTranslation.LanguageID.ToString()
        //        ).ToLower();

        //    return Managers.HashManager.FromString(str);
        //}

        #region Word

        //public static int GetPhraseHash(this Word word, string text = null)
        //{
        //    if (string.IsNullOrEmpty(word.Text))
        //    {
        //        return 0;
        //    }

        //    var result = word.Text;
        //    if (!String.IsNullOrEmpty(word.Pronunciation) && text.IndexOf("Pron") < 0)
        //    {
        //        result = $"{word.Text.Trim()} {{\"Pron\":\"{word.Pronunciation}\"}}";
        //    }

        //    var str = (result
        //        + word.LanguageID.ToString()
        //        ).ToLower();

        //    return Managers.HashManager.FromString(str);
        //}

        //public static int GetPhraseHashText(this Word word, string text = null)
        //{
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        return 0;
        //    }

        //    var str = (text.Trim()
        //        + word.LanguageID.ToString()
        //        ).ToLower();

        //    return Managers.HashManager.FromString(str);
        //}
        #endregion Word

        #region WordPhrase

        //public static int GetPhraseHash(this WordPhrase wordPhrase, string text = null)
        //{
        //    if (string.IsNullOrEmpty(wordPhrase.Text))
        //    {
        //        return 0;
        //    }

        //    var str = (wordPhrase.Text.Trim()
        //        + wordPhrase.LanguageID.ToString()
        //        ).ToLower();

        //    return Managers.HashManager.FromString(str);
        //}

        //public static int GetPhraseHashText(this WordPhrase wordPhrase, string text = null)
        //{
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        return 0;
        //    }

        //    var str = (text.Trim()
        //        + wordPhrase.LanguageID.ToString()
        //        ).ToLower();

        //    return Managers.HashManager.FromString(str);
        //}

        #endregion WordPhrase

        //public static int GetPhraseHash(this WordPhraseTranslation wordPhraseTranslation, string text = null)
        //{
        //    if (string.IsNullOrEmpty(wordPhraseTranslation.Text))
        //    {
        //        return 0;
        //    }

        //    var str = (wordPhraseTranslation.Text.Trim()
        //        + wordPhraseTranslation.LanguageID.ToString()
        //        ).ToLower();

        //    return Managers.HashManager.FromString(str);
        //}


        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: return input;
                case "": return input;
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
        public static string LastCharToDot(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            switch (input.Trim().Last())
            {
                case '.': return input;
                case '?': return input;
                case '!': return input;
                default: return input.Trim() + ".";
            }
        }
        public static string CutWithTail(this string input, int len = 100)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            var ar = input.Split(' ');
            int curLen = 0;
            foreach (var item in ar)
            {
                curLen += (item.Length + 1);
                if (curLen >= len)
                {
                    return input.Substring(0, curLen) + " ...";
                }
            }

            return input;
        }

        public static string ToOneSpace(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            var ar = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', ar);
        }

    }
}
