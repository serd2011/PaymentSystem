using System.ComponentModel.DataAnnotations;

namespace API.v1.Models.Payments
{
    public class GetRequest
    {
        public uint? limit { get; set; }
        public string? cursor { get; set; }
    }

    public class GetResponse
    {
        public List<Payment> operations;
        public string cursor;
    }

    public class Payment
    {
        public int id;
        public string description;
        public DateTime date;
        public uint amount;
        public bool isPaid;
        public int? recipient;
        public int? sender;
    }

    public class PostRequest
    {
        [Required]
        public int? id { get; set; }
        [Required]
        public uint? amount { get; set; }
    }

    public class PostResponse
    {
    }
}
