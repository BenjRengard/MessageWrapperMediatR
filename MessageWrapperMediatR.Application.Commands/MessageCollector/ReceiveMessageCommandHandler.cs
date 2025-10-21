using MediatR;
using MessageWrapperMediatR.Application.Commands.Services;
using MessageWrapperMediatR.Core.Models;
using MessageWrapperMediatR.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Application.Commands.MessageCollector
{
    public class ReceiveMessageCommandHandler : IRequestHandler<ReceiveMessageCommand>
    {
        private readonly ICollectedMessageRepository _collectedMessageRepository;
        private readonly ILogger<ReceiveMessageCommandHandler> _logger;
        private readonly IMediator _mediator;
        private readonly ICommandMessageBuilderService _commandMessageBuilderService;

        public ReceiveMessageCommandHandler(
            ICollectedMessageRepository mmessageCollectorRepository,
            ILogger<ReceiveMessageCommandHandler> logger,
            IMediator mediator,
            ICommandMessageBuilderService commandMessageBuilderService)
        {
            _collectedMessageRepository = mmessageCollectorRepository;
            _logger = logger;
            _mediator = mediator;
            _commandMessageBuilderService = commandMessageBuilderService;
        }

        public async Task Handle(ReceiveMessageCommand request, CancellationToken cancellationToken)
        {
            CollectedMessage collectedMessage = null;
            _logger.LogInformation(
                "Receive message: {message} from queue {queue}",
                request.MessageMetadata.MessageContent,
                request.MessageMetadata.HandlerData.Queue);
            if (request.MessageMetadata.HandlerData.MessageIsStored)
            {
                collectedMessage = await _collectedMessageRepository
                    .AddCollectedMessageAsync(request.MessageMetadata.MessageContent, request.MessageMetadata.HandlerData);
            }
            if (!string.IsNullOrWhiteSpace(request.MessageMetadata.HandlerData.AssociateCommand))
            {
                try
                {
                    var command = _commandMessageBuilderService.GetSpecificCommand(request.MessageMetadata);
                    if (command != null)
                    {
                        await _mediator.Send(command, cancellationToken);
                    }
                    if (collectedMessage != null)
                    {
                        await _collectedMessageRepository.UpdateStatusOfCollectedMessagesAsync(new List<Guid> { collectedMessage.Id });
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        e,
                        "Error during reception of  message: {message} from queue {queue}",
                        request.MessageMetadata.MessageContent,
                        request.MessageMetadata.HandlerData.Queue);
                }
            }
        }

    }
}
