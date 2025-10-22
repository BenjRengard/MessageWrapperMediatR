using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Application.Extensions
{
    public static class MessageWrapperMediatRApplicationExtensions
    {
        public static IServiceCollection AddMessagingWithMediatR(this IServiceCollection services/*, Assembly applicationAssembly*/)
        {
            _ = services.AddMediatR(new MediatRServiceConfiguration().RegisterServicesFromAssembly(typeof(MessageWrapperMediatRApplicationExtensions).Assembly));
            //_ = services.AddCommandMessagingAssembly(applicationAssembly);
            return services;
        }

        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(MessageWrapperMediatRApplicationExtensions).Assembly);
            return services;
        }
    }
}
