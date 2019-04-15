using Newtonsoft.Json;

namespace Satellite.NET.Models
{
    public class MessageModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
