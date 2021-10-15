using System;

namespace PaymentGateway.Abstractions
//{
////interfata generica: <T>
//    public interface  IRequest<TCommand>
//    {
//        void PerformOperation(TCommand operation);
//    }
//}
{
    public interface ITest<TCommand> // IRequest<TCommand>
    {
        void PerformOperation(TCommand operation);
    }
}