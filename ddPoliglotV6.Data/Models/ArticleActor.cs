using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddPoliglotV6.Data.Models
{
    public class ArticleActor
    {
        public int ArticleActorID { get; set; }
        public Guid KeyGuid { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public int Role { get; set; } // 0 text speach, 1- dictor voice
        public bool DefaultInRole { get; set; }
        [MaxLength(10)]
        public string Language { get; set; }
        [MaxLength(50)]
        public string VoiceName { get; set; }
        public decimal VoiceSpeed { get; set; }
        public decimal VoicePitch { get; set; }

        public int ArticleID { get; set; }
        [ForeignKey("ArticleID")]
        public Article Article { get; set; }
    }
}
