using AzureFunction.MessageQueue.DTO;

namespace AzureFunction.MessageQueue.Service
{
    public interface IProducerService
    {
        public Task SendMessageAsync<T>(Message<T> message, CancellationToken cancellationToken = default);
    }
}