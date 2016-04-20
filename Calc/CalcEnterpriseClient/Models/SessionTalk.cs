using Newtonsoft.Json;

namespace CalcEnterpriseClient.Models
{
    public class SessionTalk
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        [JsonProperty(PropertyName = "speakers")]
        public string[] Speakers { get; set; }
    }
}