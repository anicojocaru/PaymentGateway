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
//using static PaymentGateway.PublishLanguage.Commands.MultiplePurchaseCommand;
using System.Collections.Generic;
using FluentValidation;
using MediatR.Pipeline;
using PaymentGateway.WebApi.MediatorPipeline;
using PaymentGateway.PublishLanguage.Events;

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
               //services.AddMediatR(typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly);
            services.RegisterBusinessServices(Configuration);
               //services.AddSingleton<IEventSender, EventSender>();
            services.Scan(scan => scan
                .FromAssemblyOf<ListOfAccounts>()
                .AddClasses(classes => classes.AssignableTo<IValidator>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddMediatR(new[] { typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly }); // get all IRequestHandler and INotificationHandler classes

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));

            //services.AddScopedContravariant<INotificationHandler<INotification>, AllEventsHandler>(typeof(CustomerEnrolled).Assembly);

            services.AddSingleton(Configuration);

         // build
            var serviceProvider = services.BuildServiceProvider();
            var database = serviceProvider.GetRequiredService<Database>();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

         // use
         //ENROLL CUSTOMER USE CASE 
            var enrollCustomer = new EnrollCustomerCommand
            {
                ClientType = "Individual",
                AccountType = "Debit",
                Name = "Gigi Popa",
                Currency = "EUR",
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
                Currency = "EUR"
            };
            var createAccount2 = new CreateAccountCommand
            {
                PersonUniqueIdentifier = "23",
                Type = "Debit",
                Currency = "RON"
            };
            //var makeAccountOperation = serviceProvider.GetRequiredService<CreateAccount>();
            //makeAccountOperation.Handle(makeAccountDetails, default).GetAwaiter().GetResult();

            await mediator.Send(createAccountDetails, cancellationToken);
            //await mediator.Send(createAccount2, cancellationToken);

            //////////////////////////////////////////////////////////////
            //DEPOSIT MONEY useCase
            var makeDeposit = new DepositMoneyCommand
            {
                IbanOfAccount = createAccountDetails.Iban,
                Ammount = 230,
                Currency = "EUR",
                Date = DateTime.UtcNow
            };
            var makeDeposit2 = new DepositMoneyCommand
            {
                IbanOfAccount = createAccount2.Iban,
                Ammount = 500,
                Currency = "RON",
                Date = DateTime.UtcNow
            };

            //var makeDeposit = serviceProvider.GetRequiredService<DepositMoney>();
            //makeDeposit.Handle(depositDetails, default).GetAwaiter().GetResult();
            await mediator.Send(makeDeposit, cancellationToken);
            //await mediator.Send(makeDeposit2, cancellationToken);

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
            //var produs = new Product
            //{
            //    Id = 1,
            //    Limit = 10,
            //    Name = "Pantofi",
            //    Currency = "Eur",
            //    Value = 10
            //};

            //var produs1 = new Product
            //{
            //    Id = 2,
            //    Limit = 5,
            //    Name = "pantaloni",
            //    Currency = "Eur",
            //    Value = 5
            //};

            //var produs2 = new Product
            //{
            //    Id = 3,
            //    Limit = 3,
            //    Name = "Camasa",
            //    Currency = "Eur",
            //    Value = 3
            //};

            //database.Products.Add(produs);
            //database.Products.Add(produs1);
            //database.Products.Add(produs2);

            //var listaProduse = new List<CommandDetails>();

            //var prodCmd1 = new CommandDetails
            //{
            //    ProductId = 1,
            //    Quantity = 2
            //};
            //listaProduse.Add(prodCmd1);

            //var prodCmd2 = new CommandDetails
            //{
            //    ProductId = 2,
            //    Quantity = 3
            //};
            //listaProduse.Add(prodCmd2);

            //var comanda = new PurchaseProductCommand
            //{
            //    Details = listaProduse,
            //    IbanOfAccount = createAccount2.Iban
            //};

            //var query = new Application.Queries.ListOfAccounts.Query
            //{
            //    PersonId = 1
            //};

            //var handler = serviceProvider.GetRequiredService<ListOfAccounts.QueryHandler>();
            //var result = handler.Handle(query, default).GetAwaiter().GetResult();
            //var result = await mediator.Send(query, cancellationToken);


        }
    }
}
