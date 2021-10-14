using MediatR;
using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.Commands
{
    public class DepositMoneyCommand:IRequest
    {
        public double Ammount { get; set; }
        public string Currency { get; set; }
        public string IbanOfAccount { get; set; }
        public DateTime Date { get; set; }
       
    }
}
