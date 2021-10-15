using MediatR;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PaymentGateway.PublishLanguage.Commands;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateAccountOperation : IRequestHandler<CreateAccountCommand>
    {
        //private readonly IEventSender _eventSender;
        private readonly IMediator _mediator;
        private readonly AccountOptions _accountOptions;
        private readonly Database _database;
        public CreateAccountOperation(IMediator mediator, AccountOptions accountOptions, Database database)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _database = database;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var random = new Random();
            Account account = new Account();
            account.Iban = string.IsNullOrEmpty(request.Iban) ? random.Next(100000).ToString() : request.Iban;
            account.Limit = request.Limit;
            account.Balance = request.Balance;
            account.Currency = request.Currency;
            account.Id = _database.Accounts.Count + 1;

            var person = _database.Persons.FirstOrDefault(x => x.Cnp == request.PersonUniqueIdentifier);
            //if (request.PersonId.HasValue)
            //{
            //    person = _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId); //person id
            //}
            //else
            //{
            //    person = _database.Persons.FirstOrDefault(x => x.Cnp == request.PersonUniqueIdentifier); //cnp
            //}
            //if (person == null)
            //{
            //    throw new Exception("Person not found");
            //}

            _database.Accounts.Add(account);
            _database.SaveChanges();

            AccountCreated eventAccountCreated = new(request.Iban, request.Type, request.Balance, request.PersonUniqueIdentifier);
            await _mediator.Publish(eventAccountCreated, cancellationToken);
            return Unit.Value;
        }

    }
}
