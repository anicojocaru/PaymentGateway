using System;

namespace PaymentGateway.Abstractions
{
    //interfata generica: <T>
    public interface IWriteOperation<T>
    {
        void PerformOperation(T operation);
    }
}
