using MediatR;
using MessageWrapperMediatR.Core.Models;
using MessageWrapperMediatR.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Application.Commands.MessageCollector.Commands
{
    internal class PurgeMessagesCommandHandler : IRequestHandler<PurgeMessagesCommand, bool>
    {
        private readonly ICollectedMessageRepository _collectedMessageRepository;

        private readonly ILogger<PurgeMessagesCommandHandler> _logger;

        public PurgeMessagesCommandHandler(ILogger<PurgeMessagesCommandHandler> logger, ICollectedMessageRepository messageCollectorRepository)
        {
            _logger = logger;
            _collectedMessageRepository = messageCollectorRepository;
        }

        public async Task<bool> Handle(PurgeMessagesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;
                List<CollectedMessage> messagesToPurge = await _collectedMessageRepository.GetMessagesToPurgeAsync(now);
                // Vérification s'il y a des messages à purger
                if (messagesToPurge.Count == 0)
                {
                    return false;
                }
                else
                {
                    // Suppression des messages
                    await _collectedMessageRepository.DeleteMessagesAsync(messagesToPurge);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception spécifique de la couche Repository
                _logger.LogError(ex, "Une erreur s'est produite lors de la purge des messages.");
                return false;
            }
        }
    }
}
