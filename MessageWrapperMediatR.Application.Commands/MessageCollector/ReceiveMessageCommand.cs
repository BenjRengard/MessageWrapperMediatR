using MediatR;
using MessageWrapperMediatR.Core.Contracts;

namespace MessageWrapperMediatR.Application.Commands.MessageCollector
{
    public class ReceiveMessageCommand : IRequest
    {
        public MessageReceivedData MessageMetadata { get; set; }

        public ReceiveMessageCommand(MessageReceivedData messageMetadata) => this.MessageMetadata = messageMetadata;
    }
}
