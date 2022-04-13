using Newtonsoft.Json;

namespace API.v1.Models
{
    public class ErrorResponseModel
    {
        public int code;
        public string description = "";
    }

    public class InvalidModelError : ErrorResponseModel
    {
        [JsonProperty("errors", NullValueHandling = NullValueHandling.Include)]
        public Dictionary<string, List<string>> List { get; set; }
    }
}
