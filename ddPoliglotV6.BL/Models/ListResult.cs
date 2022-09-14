using Newtonsoft.Json;
using System.Collections.Generic;

namespace ddPoliglotV6.BL.Models
{
    public class ListResult<T>
    {
        public int Count { get; set; }

        public IList<T> Data { get; set; }
    }
}
