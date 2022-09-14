using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddPoliglotV6.Data.Models
{
    public class SpeechFile
    {
        public int SpeechFileID { get; set; }
        public Int64 HashCode { get; set; }
        public string SpeechFileName { get; set; }
        public int Duration { get; set; }
        public int Version { get; set; }
    }
}
