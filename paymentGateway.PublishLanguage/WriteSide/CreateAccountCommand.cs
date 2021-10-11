using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.WriteSide
{
    public class CreateAccountCommand
    {
        public double Balance { get; set; } = 0;
        public string Currency { get; set; }
        public string Iban { get; set; }
        public string Type { get; set; }
        public int Limit { get; set; }
        public string PersonUniqueIdentifier { get; set; }
        public int? PersonId { get; set; } //nullable<int> 
    }
}
