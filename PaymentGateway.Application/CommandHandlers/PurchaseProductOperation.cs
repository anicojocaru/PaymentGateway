using MediatR;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishLanguage.Commands;
using PaymentGateway.PublishLanguage.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class PurchaseProductOperation : IRequestHandler<PurchaseProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public PurchaseProductOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }
        public Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {

            ProductXTransaction pxt = new ProductXTransaction();

            Transaction transaction = new Transaction();
            Account account = new Account();
            account = _database.Accounts.FirstOrDefault(x => x.Iban == request.IbanOfAccount);

            if (account == null)
            {
                throw new Exception("Invalid Account");
            }
            double total = 0;
            foreach (var item in request.Details)
            {
                Product product = _database.Products.FirstOrDefault(x => x.Id == item.ProductId);

                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Product not in stock");
                }
                total += product.Value * item.Quantity;

                if (account.Balance < total)
                {
                    throw new Exception("Insufficient funds");
                }

                pxt.IdProduct = product.Id;
                pxt.IdTransaction = transaction.Id;
                pxt.Quantity = item.Quantity;


                product.Limit -= item.Quantity;

                _database.ProductsXTransactions.Add(pxt);
            }

        
        _database.SaveChanges();
        return Unit.Task;
          
        }
    }
}
