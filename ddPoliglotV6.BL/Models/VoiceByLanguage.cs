using System;
using System.Collections.Generic;
using System.Text;

namespace ddPoliglotV6.BL.Models
{
    public class VoiceByLanguage
    {
        public string ln { get; set; }
        public string voice { get; set; }
        public bool def { get; set; }
        public bool defTrans { get; set; }
        public bool defDict { get; set; }
        public bool defDictTransl { get; set; }
        public string sex { get; set; }
        public string namePrefix { get; set; }
        public double pitch { get; set; }
        public double speed { get; set; }
        public int languageID { get; set; }
    }
}
