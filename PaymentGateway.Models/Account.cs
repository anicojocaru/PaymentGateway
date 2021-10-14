using System;

namespace PaymentGateway.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public double Balance { get; set; } = 0;
        public string Currency { get; set; }
        public string Iban { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int Limit { get; set; }
     
    }
}
