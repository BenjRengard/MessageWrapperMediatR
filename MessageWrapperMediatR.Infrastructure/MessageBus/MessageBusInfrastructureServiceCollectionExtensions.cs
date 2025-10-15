using MessageWrapperMediatR.Domain.Factories;
using MessageWrapperMediatR.Infrastructure.MessageBus.Publisher;
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
        public static IServiceCollection AddMessageSystem(this IServiceCollection services/*, IConfiguration configuration*/)
        {
                _ = services.AddSingleton<IPublisherFactory, PublisherFactory>();

            return services;
        }
    }
}
