using AutoMapper;
using MediatR;
using MessageWrapperMediatR.Domain.Factories;
using MessageWrapperMediatR.Domain.Models;

namespace MessageWrapperMediatR.Application.Publisher
{
    public class DirectPublishMessageCommandHandler : IRequestHandler<DirectPublishMessageCommand, bool>
    {
        private readonly IPublishFactory _publisherFactory;

        private readonly IMapper _mapper;

        public DirectPublishMessageCommandHandler(IPublishFactory publisherFactory, IMapper mapper)
        {
            _publisherFactory = publisherFactory;
            _mapper = mapper;
        }

        public async Task<bool> Handle(DirectPublishMessageCommand request, CancellationToken cancellationToken)
        {
            return await _publisherFactory.PublishMessageAsync(
                _mapper.Map<MessageBusEnum>(request.BusToPublish), request.Endpoint, request.MessageContentJson, request.OptionnalRoutingKey);
        }
    }
}
