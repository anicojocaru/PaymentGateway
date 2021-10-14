using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paymentgateway.ExternalService;
using PaymentGateway.Abstractions;
using PaymentGateway.Application;
using PaymentGateway.Application.Queries;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Commands;
using System;
using System.IO;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using PaymentGateway.Data;


namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
        { 
         //partea de Web api
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

         // setup
            var services = new ServiceCollection();
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;

            services.AddMediatR(typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly);
            services.RegisterBusinessServices(Configuration);

            //services.AddSingleton<IEventSender, EventSender>();
            services.AddSingleton(Configuration);

         // build
            var serviceProvider = services.BuildServiceProvider();
            var database = serviceProvider.GetRequiredService<Database>();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

         // use
         //ENROLL CUSTOMER USE CASE 
            var enrollCustomer = new EnrollCustomerCommand
            {
                ClientType = "Company",
                AccountType = "Debit",
                Name = "Gigi Popa",
                Currency = "Eur",
                UniqueIdentifier = "23"
            };

            //var enrollCustomerOperation = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            //await enrollCustomerOperation.Handle(enrollCustomer, default);

            await mediator.Send(enrollCustomer, cancellationToken);

            /////////////////////////////////////////////////////////
            //CREATE ACCOUNT useCase
            //CreateAccountCommand account = new CreateAccountCommand();
            //account.Balance = 0;
            //account.Currency = "RON";
            //account.Iban = "ROBTRL124568584903";
            //account.Type = "Credit";
            //account.Limit = 100000000;
            //account.PersonUniqueIdentifier = enrollCustomer.UniqueIdentifier;

            //CreateAccountOperation createAccountOperation = new CreateAccountOperation(eventSender, null);
            //createAccountOperation.PerformOperation(account);

            var createAccountDetails = new CreateAccountCommand
            {
                PersonUniqueIdentifier = "23",
                Type = "Debit",
                Currency = "Eur"
            };
            //var makeAccountOperation = serviceProvider.GetRequiredService<CreateAccount>();
            //makeAccountOperation.Handle(makeAccountDetails, default).GetAwaiter().GetResult();

            await mediator.Send(createAccountDetails, cancellationToken);

            //////////////////////////////////////////////////////////////
            //DEPOSIT MONEY useCase
            var makeDeposit = new DepositMoneyCommand
            {
                IbanOfAccount = createAccountDetails.Iban,
                Ammount = 23,
                Currency = "Eur",
                Date = DateTime.UtcNow
            };

            //var makeDeposit = serviceProvider.GetRequiredService<DepositMoney>();
            //makeDeposit.Handle(depositDetails, default).GetAwaiter().GetResult();
            await mediator.Send(makeDeposit, cancellationToken);
           
            /////////////////////////////////////////////////
            ///MAKE WITHDRAW use case
            var makeWithdraw = new WithdrawMoneyCommand
            {
                Ammount = 150,
                Currency = "EUR",
                Date=DateTime.UtcNow,
                IbanOfAccount=createAccountDetails.Iban
            };

            //var makeWithdraw = serviceProvider.GetRequiredService<WithdrawMoney>();
            //makeWithdraw.Handle(withdrawDetails, default).GetAwaiter().GetResult();
            await mediator.Send(makeWithdraw, cancellationToken);

            /////////////////////////////////////////////////////////////
            //PURCHASE PRODUCT useCase
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

            var query = new Application.Queries.ListOfAccounts.Query
            {
                PersonId = 1
            };

            //var handler = serviceProvider.GetRequiredService<ListOfAccounts.QueryHandler>();
            //var result = handler.Handle(query, default).GetAwaiter().GetResult();
            var result = await mediator.Send(query, cancellationToken);


        }
    }
}
