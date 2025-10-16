namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    public class FakeRabbitChannelReceiveFactory : IRabbitChannelReceiveFactory
    {
        public Task CancelConsumptionAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task RegisterConsumptionAsync(string key, string queueName, Func<string, Task> executionMethod, List<(string routingKey, string exchange)> bindings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RestartConnectionAsync()
        {
            throw new NotImplementedException();
        }
    }
}
