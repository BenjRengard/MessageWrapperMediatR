using MessageWrapperMediatR.Core.Attributes;
using MessageWrapperMediatR.Core.Config;
using MessageWrapperMediatR.Core.Contracts;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace MessageWrapperMediatR.Application.Commands.Services
{
    public class CommandMessageBuilderService : ICommandMessageBuilderService
    {
        private readonly ILogger<CommandMessageBuilderService> _logger;
        private readonly MessageWrapperSystemConfig _config;

        public CommandMessageBuilderService(ILogger<CommandMessageBuilderService> logger, MessageWrapperSystemConfig config)
        {
            _logger = logger;
            _config = config;
        }

        public CommandMessageBase GetSpecificCommand(MessageReceivedData messageData)
        {
            Type concreteType = _config.Assembly
                .GetTypes()
                .FirstOrDefault(x =>
                    x.IsDefined(typeof(CommandMessageAttribute), false) &&
                    x.GetCustomAttribute<CommandMessageAttribute>().CommandName == messageData.HandlerData.AssociateCommand);

            if (concreteType == null)
            {
                concreteType = _config.Assembly.GetTypes().FirstOrDefault(x => x.Name == messageData.HandlerData.AssociateCommand);
            }

            if (concreteType == null)
            {
                _logger.LogError("No concrete type found for command {commandName}", messageData.HandlerData.AssociateCommand);
                return null;
            }
            var commandMessage = (CommandMessageBase)Activator.CreateInstance(concreteType);
            commandMessage.InitializeData(messageData);
            return commandMessage;
        }
    }
}
