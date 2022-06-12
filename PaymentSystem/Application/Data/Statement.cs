using System;
using System.Collections.Generic;

namespace DAL {
    public partial class Statement
    {
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
