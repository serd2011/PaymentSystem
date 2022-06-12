namespace Application.DTO
{
    public class User
    {
        public int id { get; set; }
        public uint balance { get; set; }
    }

    public class ModifyBalanceRequest
    {
        public int id { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public string idempotencyKey { get; set; }
    }
}
