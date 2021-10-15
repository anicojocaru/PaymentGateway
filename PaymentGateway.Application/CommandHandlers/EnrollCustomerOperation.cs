using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Events;
using PaymentGateway.PublishLanguage.Commands;
using MediatR;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;
        public EnrollCustomerOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {
           
            var random = new Random();

            //Database database = Database.GetInstance(); //singleton

            Person person = new Person();

            person.Cnp = request.UniqueIdentifier;
            person.Name = request.Name;

            //person.id=_database.Persons.Count+1;

            if (request.ClientType == "Individual")
            {
                person.TypeOfPerson = PersonType.Individual;
            }
            else if(request.ClientType=="Company")
            {
                person.TypeOfPerson = PersonType.Company;
            }
            else
            {
                throw new Exception("Usupported person type");
            }

            _database.Persons.Add(person);

            Account account = new Account();
            account.Type = request.AccountType;
            account.Currency = request.Currency;
            account.Balance = 0;
            account.Iban = random.Next(100000).ToString();

            _database.Accounts.Add(account);

            _database.SaveChanges();

            CustomerEnrolled eventCustomerEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType);
            await _mediator.Publish(eventCustomerEnroll, cancellationToken);
            return Unit.Value;
        }

     
    }
}
