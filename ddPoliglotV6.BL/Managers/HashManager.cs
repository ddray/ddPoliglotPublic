using System;

namespace ddPoliglotV6.BL.Managers
{
    public static class HashManager
    {
        internal static int FromString(string str)
        {
            var hash = 0;
            var arr = str.ToCharArray();
            for (var i = 0; i < arr.Length; i++)
            {
                var character = arr[i];
                hash = ((hash << 5) - hash) + character;
                hash = hash & hash; // Convert to 32bit integer
            }

            return hash;
        }

        public static int GetHashFromTextAndVoice(this string text, string ln, string voiceName, decimal pitch, decimal speed)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var str = (text.Trim()
                + ln.Trim()
                + voiceName.Trim()
                + pitch.ToString("#.##").Trim()
                + speed.ToString("#.##")
                ).ToLower();

            return FromString(str);
        }

        public static int GetHashCodeFromFileName(string textSpeechFileName)
        {
            var result = -1;
            if (!string.IsNullOrEmpty(textSpeechFileName))
            {
                var ar = textSpeechFileName.Split("_");
                if (ar.Length > 0)
                {
                    result = Convert.ToInt32(ar[0].Replace("n", "-"));
                }
            }

            return result;
        }
    }
}
