namespace PaymentValidator.API.Interfaces
{
    public interface ILogger<TMessage>
    {
        Task LogAsync(TMessage message);
    }
}
