using MediatR;
using MessageWrapperMediatR.Core.Contracts;

namespace MessageWrapperMediatR.Application.Commands
{
    public abstract class CommandMessageBase : IRequest
    {
        public MessageReceivedData MessageMetadata { get; set; }

        public virtual void InitializeData(MessageReceivedData messageReceivedData)
        {
            MessageMetadata = messageReceivedData;
        }
    }
}
