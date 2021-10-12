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
    public class PurchaseProductOperation : IWriteOperation<PurchaseProductCommand>
    {
        public IEventSender eventSender;
        public PurchaseProductOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(PurchaseProductCommand operation)
        {
            Database database = Database.GetInstance();

            ProductXTransaction pxt = new ProductXTransaction();

            Transaction transaction = new Transaction();
            Product product = new Product();
            Account account = new Account();

            //account=database.Accounts.FirstOrDefault(x => x.Iban == operation.IbanOfAccount);
            product = database.Products.FirstOrDefault(x => x.Name == operation.Name);

            pxt.IdProduct = product.Id;
            pxt.IdTransaction = transaction.Id;

            if(operation.Value > account.Balance)
            {
                throw new Exception("Fond insuficient");
            }
            if (operation.Limit > product.Limit)
            {
                throw new Exception("Nu avem atatea produse disponibile");
            }
            product.Limit = operation.Limit;
            product.Name = operation.Name;
            product.Value = operation.Value;
            product.Currency = operation.Currency;
           

            database.Transactions.Add(transaction);
            database.ProductsXTransactions.Add(pxt);

            
            database.SaveChanges();
           

            ProductPurchased eventProductPurchased = new(operation.Name, operation.Value, operation.Currency, operation.Limit);
            eventSender.SendEvent(eventProductPurchased);
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
