using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishLanguage.WriteSide
{
    public class WithdrawMoneyCommand
    {
        public double Ammount { get; set; }
        public string Currency { get; set; }
        public string IbanOfAccount { get; set; }
        public DateTime Date { get; set; }
    }
}
