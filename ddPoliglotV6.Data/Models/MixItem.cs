
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ddPoliglotV6.Data.Models
{
    public class MixItem
    {
        public int MixItemID { get; set; }

        public int MixParamID { get; set; }

        [MaxLength(50)]
        public string KeyGuid { get; set; }

        [MaxLength(200)]
        public string Source { get; set; }

        [MaxLength(300)]
        public string InDict { get; set; }

        [MaxLength(300)]
        public string InContext { get; set; }

        [MaxLength(50)]
        public string ChildrenType { get; set; }

        public bool EndPhrase { get; set; }
        public bool Pretext { get; set; }
        public int OrderNum { get; set; }
        public bool baseWord { get; set; }
    }
}
