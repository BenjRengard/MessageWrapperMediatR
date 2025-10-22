using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Core.Contracts
{
    public class MessageReceivedData
    {
        public string MessageContent { get; set; }

        public Handler HandlerData { get; set; }

        public MessageReceivedData(string messageContent, Handler handlerData)
        {
            MessageContent = messageContent;
            HandlerData = handlerData;
        }
    }
}
