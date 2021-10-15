using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Events;
using MediatR;
using PaymentGateway.PublishLanguage.Commands;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class WithdrawMoneyOperation : IRequest<WithdrawMoneyCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public WithdrawMoneyOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }
        public async Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();

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

            if(account.Balance< request.Ammount)
            {
                throw new Exception("Fonduri insuficiente");
            }
            else
            {
                account.Balance -= request.Ammount;
            }
            _database.SaveChanges();

            AccountUpdated eventAccountUpdated = new(request.Date, request.Ammount, request.IbanOfAccount);
            await _mediator.Publish(eventAccountUpdated, cancellationToken);
            return Unit.Value;
        }
    }
}
