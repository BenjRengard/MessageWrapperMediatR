using AutoMapper;
using MediatR;
using MessageWrapperMediatR.Domain.Factories;
using MessageWrapperMediatR.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Application.Publisher
{
    public class DirectPublishMessageCommandHandler : IRequestHandler<DirectPublishMessageCommand, bool>
    {
        private readonly IPublisherFactory _publisherFactory;

        private readonly IMapper _mapper;

        public DirectPublishMessageCommandHandler(IPublisherFactory publisherFactory, IMapper mapper)
        {
            _publisherFactory = publisherFactory;
            _mapper = mapper;
        }

        public async Task<bool> Handle(DirectPublishMessageCommand request, CancellationToken cancellationToken)
        {
            //return await _publisherFactory.PublishMessageAsync(MessageBusEnum.rabbitmq, request.Endpoint, request.MessageContentJson, request.OptionnalRoutingKey);
            return await _publisherFactory.PublishMessageAsync(_mapper.Map<MessageBusEnum>(request.BusToPublish), request.Endpoint, request.MessageContentJson, request.OptionnalRoutingKey);
        }
    }
}
