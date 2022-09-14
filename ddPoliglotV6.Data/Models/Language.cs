using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ddPoliglotV6.Data.Models
{
    public class Language
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int LanguageID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(10)]
        public string CodeFull { get; set; }
        [MaxLength(50)]
        public string ShortName { get; set; }
    }
}
