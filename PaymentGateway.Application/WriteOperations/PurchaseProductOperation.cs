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
    public class PurchaseProductOperation : IRequest<PurchaseProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public PurchaseProductOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }
        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {
            

            ProductXTransaction pxt = new ProductXTransaction();

            Transaction transaction = new Transaction();
            Product product = new Product();
            Account account = new Account();

            //account=database.Accounts.FirstOrDefault(x => x.Iban == operation.IbanOfAccount);
            product = _database.Products.FirstOrDefault(x => x.Name == request.Name);

            pxt.IdProduct = product.Id;
            pxt.IdTransaction = transaction.Id;

            if(request.Value > account.Balance)
            {
                throw new Exception("Fond insuficient");
            }
            if (request.Limit > product.Limit)
            {
                throw new Exception("Nu avem atatea produse disponibile");
            }
            product.Limit = request.Limit;
            product.Name = request.Name;
            product.Value = request.Value;
            product.Currency = request.Currency;
           

            _database.Transactions.Add(transaction);
            _database.ProductsXTransactions.Add(pxt);

            
            _database.SaveChanges();
           

            ProductPurchased eventProductPurchased = new(request.Name, request.Value, request.Currency, request.Limit);
            await _mediator.Publish(eventProductPurchased, cancellationToken);
            return Unit.Value;
        }
        class MultipleProductPurchase 
        {

            public class ProductPurchase
            {
                public int ProductId { get; set; }
                public double Quantity { get; set; }
            }
            
            public List<ProductPurchase> ListToPurchase { get; set; }
            
        }
    }
}
