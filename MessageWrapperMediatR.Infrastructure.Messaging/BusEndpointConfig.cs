using MessageWrapperMediatR.Domain.Models;

namespace MessageWrapperMediatR.Infrastructure.Messaging
{
    public class BusEndpointConfig
    {
        public required string Type { get; set; }

        public MessageBusEnum BusType { get; set; }

        public required string EndpointName { get; set; }

        public string? RoutingKey { get; set; }

    }
}
