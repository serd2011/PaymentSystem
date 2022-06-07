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
            Statements = new HashSet<Statement>();
        }

        public int Id { get; set; }

        public virtual ICollection<Payment> PaymentFroms { get; set; }
        public virtual ICollection<Payment> PaymentTos { get; set; }
        public virtual ICollection<Statement> Statements { get; set; }
    }
}
