using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Events;
using PaymentGateway.PublishLanguage.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class DepositMoneyOperation : IRequestHandler<DepositMoneyCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;
        public DepositMoneyOperation(Mediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            Transaction transaction = new Transaction();
            transaction.Ammount = request.Ammount;
            transaction.Currency = request.Currency;
            transaction.Date = DateTime.UtcNow;

            var account = _database.Accounts.FirstOrDefault(x => x.Iban == request.IbanOfAccount);
            if (account == null)
            {
                throw new Exception("contul nu exista");
            }
            if (account.Currency != request.Currency)
            {
                throw new Exception("Valuta invalida");
            }

            _database.Transactions.Add(transaction);
            account.Balance = account.Balance + request.Ammount;
            _database.SaveChanges();

            AccountUpdated eventAccountUpdated = new(request.Date, request.Ammount, request.IbanOfAccount);
            await _mediator.Publish(eventAccountUpdated, cancellationToken);
            return Unit.Value;
        }

   
    }
}
