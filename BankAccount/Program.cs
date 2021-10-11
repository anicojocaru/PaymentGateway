using Paymentgateway.ExternalService;
using PaymentGateway.Abstractions;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.NewFolder;
using PaymentGateway.PublishLanguage.WriteSide;
using System;

namespace PaymentGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            Account firstAccount = new Account();
            firstAccount.Balance = 100;


            EnrollCustomerCommand customer1 = new EnrollCustomerCommand();
            customer1.Name = "Andreea Cojocaru";
            customer1.UniqueIdentifier = "2970304234566";
            customer1.ClientType = "Individual";
            customer1.AccountType = "Debit";
            customer1.Currency = "RON";

            IEventSender eventSender = new EventSender();
            EnrollCustomerOperation enrollCustomerOperation = new EnrollCustomerOperation(eventSender);
            enrollCustomerOperation.PerformOperation(customer1);

            //create account useCase
            CreateAccountCommand account = new CreateAccountCommand();
            account.Balance = 0;
            account.Currency = "RON";
            account.Iban = "ROBTRL124568584903";
            account.Type = "Credit";
            account.Limit = 100000000;
            account.PersonUniqueIdentifier = customer1.UniqueIdentifier;

            CreateAccountOperation createAccountOperation = new CreateAccountOperation(eventSender);
            createAccountOperation.PerformOperation(account);

            //deposit money useCase
            DepositMoneyCommand depositMoney = new DepositMoneyCommand();
            depositMoney.Ammount = 200;
            depositMoney.Currency = "RON";
            depositMoney.Date = DateTime.UtcNow;
            depositMoney.IbanOfAccount = account.Iban;

            DepositMoneyOperation depositMoneyOperation = new DepositMoneyOperation(eventSender);
            depositMoneyOperation.PerformOperation(depositMoney);

            //withdraw money useCase
            WithdrawMoneyCommand withdrawMoney = new WithdrawMoneyCommand();
            withdrawMoney.Ammount = 100;
            withdrawMoney.Currency = "RON";
            withdrawMoney.Date = DateTime.UtcNow;
            withdrawMoney.IbanOfAccount = account.Iban;

            WithdrawMoneyOperation withDrawMoneyOperation = new WithdrawMoneyOperation(eventSender);
            withDrawMoneyOperation.PerformOperation(withdrawMoney);

            //purchase product useCase
            Product product = new Product();
            product.Limit = 10000;
            

            PurchaseProductCommand purchaseProduct = new PurchaseProductCommand();
            purchaseProduct.Currency = "RON";
            purchaseProduct.Value = 25;
            purchaseProduct.Limit = 10;
            purchaseProduct.Name = "produs1";
            purchaseProduct.IbanOfAccount = account.Iban;

            PurchaseProductOperation purchaseOperation = new PurchaseProductOperation(eventSender);
            purchaseOperation.PerformOperation(purchaseProduct);


        }
    }
}
