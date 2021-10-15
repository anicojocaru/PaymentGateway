using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentGateway.PublishLanguage.Commands.MultiplePurchaseCommand;

namespace PaymentGateway.PublishLanguage.Commands
{
    public class PurchaseProductCommand:IRequest
    {
        public List<CommandDetails> Details { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }
        public string Cnp { get; set; }
        public string IbanOfAccount { get; set; }   
    }
}
