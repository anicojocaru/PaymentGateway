using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.Events
{
    public class AccountUpdated
    {
        public DateTime Date { get; set; }
        public double Ammount { get; set; }
        public string Iban { get; set; }

        public AccountUpdated(DateTime data, double ammount, string iban)
        {
            this.Date = data;
            this.Ammount = ammount;
            this.Iban = iban;
        }
    }
}
