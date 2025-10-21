using MediatR;
using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Application.Publisher
{
    public class DirectPublishMessageCommand : IRequest<bool>
    {
        public MessageBusEnum BusToPublish { get; set; }

        public string Endpoint { get; set; }

        public string MessageContentJson { get; set; }

        public string? OptionnalRoutingKey { get; set; }

    }
}
