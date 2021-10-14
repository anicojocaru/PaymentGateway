using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Events;
using PaymentGateway.PublishLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateAccountOperation : IWriteOperation<CreateAccountCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly AccountOptions _accountOptions;
        private readonly Database _database;
        public CreateAccountOperation(IEventSender eventSender, AccountOptions accountOptions, Database database)
        {
            _eventSender = eventSender;
            _accountOptions = accountOptions;
            _database = database;
        }

        public void PerformOperation(CreateAccountCommand operation)
        {
            var random = new Random();

            //Database database = Database.GetInstance();
            Account account = new Account();
            account.Iban = string.IsNullOrEmpty(operation.Iban) ? random.Next(100000).ToString() : operation.Iban;
            account.Limit = operation.Limit;
            account.Balance = operation.Balance;
            account.Currency = operation.Currency;
            account.Id = _database.Accounts.Count + 1;

            var person = _database.Persons.FirstOrDefault(x => x.Cnp == operation.PersonUniqueIdentifier);
            if (operation.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.PersonId == operation.PersonId); //person id
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.Cnp == operation.PersonUniqueIdentifier); //cnp
            }
            if (person == null)
            {
                throw new Exception("Person not found");
            }

            _database.Accounts.Add(account);
            _database.SaveChanges();

            AccountCreated eventAccountCreated = new(operation.Iban, operation.Type, operation.Balance, operation.PersonUniqueIdentifier);
            _eventSender.SendEvent(eventAccountCreated);
        }
    }
}
