using AutoMapper;
using MessageWrapperMediatR.Application.Contracts;
using MessageWrapperMediatR.Domain.Models;

namespace MessageWrapperMediatR.Application.Mappers
{
    public class MessageBusMapper : Profile
    {
        public MessageBusMapper()
        {
            _ = this.CreateMap<MessageBusEnumContract, MessageBusEnum>().ReverseMap();
        }
    }
}
