using System.ComponentModel.DataAnnotations;

namespace API.v1.Models.Management
{
    public class AddRequest
    {
        [Required]
        public int? id { get; set; }
        [Required]
        public uint? amount { get; set; }
        public string? description { get; set; }
    }

    public class RemoveRequest
    {
        [Required]
        public int? id { get; set; }
        [Required]
        public uint? amount { get; set; }
        public string? description { get; set; }
    }
}
