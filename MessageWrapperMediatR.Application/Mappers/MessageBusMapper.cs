using AutoMapper;
using MessageWrapperMediatR.Application.Contracts;
using MessageWrapperMediatR.Core.Models;

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
