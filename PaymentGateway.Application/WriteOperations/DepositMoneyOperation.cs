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
    public class DepositMoneyOperation : IWriteOperation<DepositMoneyCommand>
    {
        public IEventSender eventSender;
        public DepositMoneyOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public void PerformOperation(DepositMoneyCommand operation)
        {
            Database database = Database.GetInstance();

            Transaction transaction = new Transaction();
            transaction.Ammount = operation.Ammount;
            transaction.Currency = operation.Currency;
            transaction.Date = DateTime.UtcNow;

            var account = database.Accounts.FirstOrDefault(x => x.Iban == operation.IbanOfAccount);
            if (account == null)
            {
                throw new Exception("contul nu exista");
            }
            if (account.Currency != operation.Currency)
            {
                throw new Exception("Valuta invalida");
            }

            database.Transactions.Add(transaction);
            account.Balance = account.Balance + operation.Ammount;
            database.SaveChanges();

            AccountUpdated eventAccountUpdated = new(operation.Date, operation.Ammount, operation.IbanOfAccount);
            eventSender.SendEvent(eventAccountUpdated);
        }
    }
}
