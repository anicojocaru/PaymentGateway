using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.Commands
{
    public class MultiplePurchaseCommand : IRequest
    {
        public List<CommandDetails> Details { get; set; }
        public int AccountId { get; set; }
        public class CommandDetails
        {
            public int ProductId { get; set; }
            public double Quantity { get; set; }
        }
    }
}

