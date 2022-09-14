
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ddPoliglotV6.Data.Models
{
    public class Log
    {
        public int LogID { get; set; }
        
        public Guid UserID { get; set; }
        
        [MaxLength(1000)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Message { get; set; }

        public string Description { get; set; }

        public int Type { get; set; }

        public DateTime Created { get; set; }
    }
}
