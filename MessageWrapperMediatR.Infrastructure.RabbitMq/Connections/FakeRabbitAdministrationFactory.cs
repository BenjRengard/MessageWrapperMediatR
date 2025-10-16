namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    public class FakeRabbitAdministrationFactory : IRabbitAdministrationFactory
    {
        public void PurgeAllExchangeCache()
        {
            throw new NotImplementedException();
        }

        public Task RegisterExchangeToPublishAsync(string exchange)
        {
            throw new NotImplementedException();
        }

        public Task RegisterQueueAsync(string queue, List<(string routingKey, string exchange)> bindings)
        {
            throw new NotImplementedException();
        }

        public bool RemoveExchangeInCache(string exchange)
        {
            throw new NotImplementedException();
        }
    }
}
