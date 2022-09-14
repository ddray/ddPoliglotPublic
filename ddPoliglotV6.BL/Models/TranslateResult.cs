using Newtonsoft.Json;

namespace ddPoliglotV6.BL.Models
{
    public class TranslateResult
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
