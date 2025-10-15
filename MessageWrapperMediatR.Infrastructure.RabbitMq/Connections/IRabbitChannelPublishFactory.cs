using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    public interface IRabbitChannelPublishFactory
    {
        Task<IChannel> GetChannelAsync();
    }
}
