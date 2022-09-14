using System;
using System.ComponentModel.DataAnnotations;

namespace ddPoliglotV6.Data.Models
{
    public class UserLanguageLevel
    {
        [Key]
        public int UserLanguageLevelID { get; set; }
        public int LanguageID { get; set; }
        public Guid UserID { get; set; }
        public int Level { get; set; }
    }
}
