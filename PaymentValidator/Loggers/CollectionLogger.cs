using PaymentValidator.Interfaces;

namespace PaymentValidator.Loggers
{
    public class CollectionLogger<TMessage>(ICollection<TMessage> collection) : ILogger<TMessage>
    {
		private readonly ICollection<TMessage> _collection = collection;

		public async Task LogAsync(TMessage message)
		{
			_collection.Add(message);
		}
	}
}
