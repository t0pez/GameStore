namespace GameStore.Core.Interfaces.PaymentMethods;

public interface IPaymentMethodFactory
{
    public IPaymentMethod GetPaymentMethod(PaymentType type);
}

