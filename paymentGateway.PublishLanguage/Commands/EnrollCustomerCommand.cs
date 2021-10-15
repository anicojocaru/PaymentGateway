using MediatR;
using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.Commands
{
    public class EnrollCustomerCommand : IRequest
    {
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }
        public string ClientType { get; set; }
     
        public string AccountType { get; set; }
        public string Currency { get; set; }
    }
}
