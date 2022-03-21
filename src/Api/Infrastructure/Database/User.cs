using System;
using System.Collections.Generic;

namespace API.Infrastructure.Database
{
    public partial class User
    {
        public User()
        {
            PaymentFroms = new HashSet<Payment>();
            PaymentTos = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public int Balance { get; set; }

        public virtual ICollection<Payment> PaymentFroms { get; set; }
        public virtual ICollection<Payment> PaymentTos { get; set; }
    }
}
