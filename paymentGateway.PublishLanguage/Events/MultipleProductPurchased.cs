using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.Events
{
    public class MultipleProductPurchased : INotification
    {
        public int ProductId { get; set; }
        public double Quantity { get; set; }

        public MultipleProductPurchased(int id, double q)
        {
            this.ProductId = id;
            this.Quantity = q;
        }
    }
}
