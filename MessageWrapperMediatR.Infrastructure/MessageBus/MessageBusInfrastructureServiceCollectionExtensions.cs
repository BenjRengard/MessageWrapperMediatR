using MessageWrapperMediatR.Core.Factories;
using MessageWrapperMediatR.Infrastructure.IbmMqSeries;
using MessageWrapperMediatR.Infrastructure.MessageBus.Publisher;
using MessageWrapperMediatR.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.MessageBus
{
    public static class MessageBusInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddPublisherServices(this IServiceCollection services, IConfiguration configuration)
        {
            PublisherConfig publisherConfig = configuration.GetSection("PublisherConfig").Get<PublisherConfig>() ?? new PublisherConfig();
            _ = services.AddSingleton(publisherConfig);
            _ = services.AddSingleton<IPublishFactory, PublishFactory>();
            _ = services.AddSingleton<IGenericMessagePublisher, GenericMessagePublisher>();

            return services;
        }
    }
}
