using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paymentgateway.ExternalService;
using PaymentGateway.Abstractions;
using PaymentGateway.Application;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.NewFolder;
using PaymentGateway.PublishLanguage.WriteSide;
using System;
using System.IO;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static void Main(string[] args)
        {
            Account firstAccount = new Account();
            firstAccount.Balance = 100;
            ////////////////////////////////////////////////////////
            //enroll customer useCase
            EnrollCustomerCommand customer1 = new EnrollCustomerCommand();
            customer1.Name = "Andreea Cojocaru";
            customer1.UniqueIdentifier = "2970304234566";
            customer1.ClientType = "Individual";
            customer1.AccountType = "Debit";
            customer1.Currency = "RON";

            IEventSender eventSender = new EventSender();
            EnrollCustomerOperation enrollCustomerOperation1 = new EnrollCustomerOperation(eventSender);
            enrollCustomerOperation1.PerformOperation(customer1);


            //partea de Web api
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // setup
            var services = new ServiceCollection();
            services.RegisterBusinessServices(Configuration);

            services.AddSingleton<IEventSender, EventSender>();
            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();

            // use
            var enrollCustomer = new EnrollCustomerCommand
            {
                ClientType = "Company",
                AccountType = "Debit",
                Name = "Gigi Popa",
                Currency = "Eur",
                UniqueIdentifier = "23"
            };

            var enrollCustomerOperation = serviceProvider.GetRequiredService<EnrollCustomerOperation>();

            /////////////////////////////////////////////////////////
            //create account useCase
            CreateAccountCommand account = new CreateAccountCommand();
            account.Balance = 0;
            account.Currency = "RON";
            account.Iban = "ROBTRL124568584903";
            account.Type = "Credit";
            account.Limit = 100000000;
            account.PersonUniqueIdentifier = customer1.UniqueIdentifier;

            CreateAccountOperation createAccountOperation = new CreateAccountOperation(eventSender, null);
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
            //Product product = new Product();
            //product.Limit = 10000;

            //PurchaseProductCommand purchaseProduct = new PurchaseProductCommand();
            //purchaseProduct.Currency = "RON";
            //purchaseProduct.Value = 25;
            //purchaseProduct.Limit = 10;
            //purchaseProduct.Name = "produs1";
            //purchaseProduct.IbanOfAccount = account.Iban;
            
            //PurchaseProductOperation purchaseOperation = new PurchaseProductOperation(eventSender);
            //purchaseOperation.PerformOperation(purchaseProduct);

        }
    }
}
