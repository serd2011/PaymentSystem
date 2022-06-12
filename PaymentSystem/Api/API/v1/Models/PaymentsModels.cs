using System.ComponentModel.DataAnnotations;

namespace API.v1.Models.Payments
{
    public class GetRequest
    {
        [Range(1, 100)]
        public uint limit { get; set; } = 10;
        public int? cursor { get; set; }
    }

    public class PostRequest
    {
#if !USE_AUTHENTICATION
        [Required]
        public int? fromId { get; set; }
#endif
        [Required]
        public int? id { get; set; }
        [Required]
        public uint? amount { get; set; }
        public string? description { get; set; }
    }
}
