namespace Application.DTO
{
    public class PaymentsServiceDTOs
    {
        public IEnumerable<Payment> operations { get; set; }
        public int? cursor { get; set; }
    }

    public class Payment
    {
        public int id { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public uint amount { get; set; }
        public bool isPaid { get; set; }
        public int userId { get; set; }
    }

    public class PaymentsRequest
    {
        public int id { get; set; }
        public uint limit { get; set; }
        public int? cursor { get; set; }
    }

    public class CreatePaymentRequest
    {
        public int fromId { get; set; }
        public int toId { get; set; }
        public uint amount { get; set; }
        public string description { get; set; }
        public string idempotencyKey { get; set; }
    }
}
