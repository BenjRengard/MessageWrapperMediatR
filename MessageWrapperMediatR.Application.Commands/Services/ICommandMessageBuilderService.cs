using MessageWrapperMediatR.Core.Contracts;

namespace MessageWrapperMediatR.Application.Commands.Services
{
    public interface ICommandMessageBuilderService
    {
        CommandMessageBase GetSpecificCommand(MessageReceivedData messageData);

    }
}
