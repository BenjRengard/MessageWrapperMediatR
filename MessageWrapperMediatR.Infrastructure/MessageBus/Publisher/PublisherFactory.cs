using MessageWrapperMediatR.Domain.Factories;
using MessageWrapperMediatR.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.MessageBus.Publisher
{
    public class PublisherFactory : IPublisherFactory
    {
        public async Task<bool> PublishMessageAsync(MessageBusEnum messageBus, string endpoint, string messageContentJson, string? routingKeyOptionnal = null)
        {
            return true;
        }
    }
}
