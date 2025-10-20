using MediatR;
using MessageWrapperMediatR.Application.Contracts;
using System.Text.Json.Serialization;

namespace MessageWrapperMediatR.Application.Publisher
{
    public class DirectPublishMessageCommand : IRequest<bool>
    {
        public MessageBusEnumContract BusToPublish { get; set; }

        public string Endpoint { get; set; }

        public string MessageContentJson { get; set; }

        public string? OptionnalRoutingKey { get; set; }

    }
}
