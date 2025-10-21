using AutoMapper;
using MessageWrapperMediatR.Core.Interfaces;
using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Core.Mappers
{
    public class HandlerMapper : Profile
    {
        public HandlerMapper()
        {
            _ = this.CreateMap<IDynamicHandler, Handler>().ForMember(dest => dest.Bindings, opt => opt.MapFrom(src => src.Bindings)).ReverseMap();
        }
    }
}
