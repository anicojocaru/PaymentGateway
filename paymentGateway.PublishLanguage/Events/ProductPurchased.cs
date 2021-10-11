using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.Events
{
    public class ProductPurchased
    { 
        public string Name { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }

        public ProductPurchased(string name, double value, string currency, double limit)
        {
            this.Name = name;
            this.Value = value;
            this.Currency = currency;
            this.Limit = limit;
        }
    }
}
