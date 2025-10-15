using MessageWrapperMediatR.Domain.Factories;
using MessageWrapperMediatR.Domain.Models;
using MessageWrapperMediatR.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Infrastructure.MessageBus.Publisher
{
    public class PublishFactory : IPublishFactory
    {
        private readonly ILogger<PublishFactory> _logger;
        private readonly IGenericMessagePublisher _messagePublisher;

        public PublishFactory(ILogger<PublishFactory> logger, IGenericMessagePublisher genericMessagePublisher)
        {
            _logger = logger;
            _messagePublisher = genericMessagePublisher;
        }

        public async Task<bool> PublishMessageAsync(MessageBusEnum messageBus, string endpoint, string messageContentJson, string? routingKeyOptionnal = null)
        {
            try
            {
                PublisherMessageEnveloppeBase<object> enveloppe = _messagePublisher.BuildEnveloppeFromMetadata(messageBus, endpoint, messageContentJson, routingKeyOptionnal);
                PublisherResult result = await _messagePublisher.DirectPublishToBusAsync(enveloppe);
                if (result != null && result.IsSuccess)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erreur lors de l'envoi du message en direct.");
            }
            return false;
        }
    }
}
