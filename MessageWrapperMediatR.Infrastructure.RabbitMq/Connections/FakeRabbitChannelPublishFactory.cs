using RabbitMQ.Client;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    public class FakeRabbitChannelPublishFactory : IRabbitChannelPublishFactory
    {
        public Task<IChannel> GetChannelAsync()
        {
            throw new NotImplementedException();
        }
    }
}
