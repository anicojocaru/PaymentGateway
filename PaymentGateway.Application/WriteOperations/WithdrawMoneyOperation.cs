﻿using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.PublishLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Events;

namespace PaymentGateway.Application.WriteOperations
{
    public class WithdrawMoneyOperation : IWriteOperation<WithdrawMoneyCommand>
    {
        public IEventSender eventSender;
        private readonly Database _database;
        public WithdrawMoneyOperation(IEventSender eventSender, Database database)
        {
            this.eventSender = eventSender;
            _database = database;
        }
        public void PerformOperation(WithdrawMoneyCommand operation)
        {
            //Database database = Database.GetInstance();

            Transaction transaction = new Transaction();
            transaction.Ammount = operation.Ammount;
            transaction.Currency = operation.Currency;
            transaction.Date = DateTime.UtcNow;

            var account = _database.Accounts.FirstOrDefault(x => x.Iban == operation.IbanOfAccount);
            if (account == null)
            {
                throw new Exception("contul nu exista");
            }
            if (account.Currency != operation.Currency)
            {
                throw new Exception("Valuta invalida");
            }

            _database.Transactions.Add(transaction);

            if(account.Balance<operation.Ammount)
            {
                throw new Exception("Fonduri insuficiente");
            }
            else
            {
                account.Balance -= operation.Ammount;
            }
            _database.SaveChanges();

            AccountUpdated eventAccountUpdated = new(operation.Date, operation.Ammount, operation.IbanOfAccount);
            eventSender.SendEvent(eventAccountUpdated);
        }
    }
}
