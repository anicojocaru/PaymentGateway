using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class ProductXTransaction
    {
        public int IdTransaction { get; set; }
        public int IdProduct { get; set; }
        public double Quantity { get; set; }
    }
}
