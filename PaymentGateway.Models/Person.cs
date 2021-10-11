using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cnp { get; set; }
        public PersonType TypeOfPerson { get; set; } //enum
        public List<Account> accounts { get; set; } = new List<Account>();
        public int PersonId { get; set; }
    }
}
