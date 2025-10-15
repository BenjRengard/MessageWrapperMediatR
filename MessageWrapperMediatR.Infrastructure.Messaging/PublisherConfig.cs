namespace MessageWrapperMediatR.Infrastructure.Messaging
{
    public class PublisherConfig
    {
        public List<BusEndpointConfig> Endpoints { get; set; } = new List<BusEndpointConfig>();

    }
}
