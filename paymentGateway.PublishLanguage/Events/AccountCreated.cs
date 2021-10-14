using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.Events
{
    public class AccountCreated:INotification
    {
        public string Iban { get; set; }
        public string Type { get; set; }
        public double Balance { get; set; }
        public string Cnp { get; set; }

        public AccountCreated(string iban, string type, double balance, string cnp)
        {
            this.Iban = iban;
            this.Type = type;
            this.Balance = balance;
            this.Cnp = cnp;


        }
    }
}
