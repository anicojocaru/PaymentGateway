using System;

namespace PaymentGateway.Abstractions
{
    //interfata generica: <T>
    public interface IWriteOperation<TCommand>
    {
        void PerformOperation(TCommand operation);
    }
}
