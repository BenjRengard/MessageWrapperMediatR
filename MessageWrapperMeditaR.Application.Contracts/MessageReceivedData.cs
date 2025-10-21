using MessageWrapperMediatR.Domain.Models;

namespace MessageWrapperMediatR.Application.Contracts
{
    public class MessageReceivedData
    {
        public string MessageContent { get; set; }

        public Handler HandlerData { get; set; }

        public MessageReceivedData(string messageContent, Handler handlerData)
        {
            this.MessageContent = messageContent;
            this.HandlerData = handlerData;
        }
    }
}
