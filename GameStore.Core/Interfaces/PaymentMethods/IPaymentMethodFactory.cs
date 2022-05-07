namespace GameStore.Core.Interfaces.PaymentMethods;

public interface IPaymentMethodFactory
{
    public IPaymentMethod Get(PaymentType type);
}