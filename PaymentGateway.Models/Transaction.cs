using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public double Ammount { get; set; }
        public DateTime Date {get; set;}
        public string Currency { get; set; }
        public string Type{ get; set; }
        
    }
}
