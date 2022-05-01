using System;
using System.Collections.Generic;

namespace API.Infrastructure.Database
{
    public partial class Payment
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public int? FromId { get; set; }
        public int? ToId { get; set; }

        public virtual User? From { get; set; }
        public virtual User? To { get; set; }
    }
}
