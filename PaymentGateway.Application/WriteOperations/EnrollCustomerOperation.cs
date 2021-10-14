using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Events;
using PaymentGateway.PublishLanguage.NewFolder;

namespace PaymentGateway.Application.WriteOperations
{
    public class EnrollCustomerOperation : IWriteOperation<EnrollCustomerCommand>
    {
        private readonly Database _database;
        public IEventSender eventSender;
        public EnrollCustomerOperation(IEventSender eventSender, Database database)
        {
            this.eventSender = eventSender;
            _database = database;
        }

        public void PerformOperation(EnrollCustomerCommand operation)
        {
           
            var random = new Random();

            //Database database = Database.GetInstance(); //singleton

            Person person = new Person();

            person.Cnp = operation.UniqueIdentifier;
            person.Name = operation.Name;

            //person.id=_database.Persons.Count+1;

            if (operation.ClientType == "Individual")
            {
                person.TypeOfPerson = PersonType.Individual;
            }
            else if(operation.ClientType=="Company")
            {
                person.TypeOfPerson = PersonType.Company;
            }
            else
            {
                throw new Exception("Usupported person type");
            }

            _database.Persons.Add(person);

            Account account = new Account();
            account.Type = operation.AccountType;
            account.Currency = operation.Currency;
            account.Balance = 0;
            account.Iban = random.Next(100000).ToString();

            _database.Accounts.Add(account);

            _database.SaveChanges();

            CustomerEnrolled eventCustomerEnroll = new(operation.Name,operation.UniqueIdentifier,operation.ClientType);
            eventSender.SendEvent(eventCustomerEnroll);
        }

      
    }
}
