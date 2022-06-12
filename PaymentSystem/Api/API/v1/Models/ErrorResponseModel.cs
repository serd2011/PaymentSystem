using Newtonsoft.Json;

namespace API.v1.Models
{
    public class ErrorResponseModel
    {
        public int code { get; set; }
        public string description { get; set; } = "";
    }

    public class InvalidModelError : ErrorResponseModel
    {
        [JsonProperty("errors", NullValueHandling = NullValueHandling.Include)]
        public Dictionary<string, List<string>> List { get; set; }
    }
}
