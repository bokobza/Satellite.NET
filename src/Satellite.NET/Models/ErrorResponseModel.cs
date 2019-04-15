using Newtonsoft.Json;

namespace Satellite.NET.Models
{
    public class ErrorResponseModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errors")]
        public ErrorModel[] Errors { get; set; }
    }

    public class ErrorModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }
}
