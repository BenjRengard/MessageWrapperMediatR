using System.Runtime.Serialization;

namespace MessageWrapperMediatR.Application.Contracts
{
    [DataContract]
    public class PagingFilterContract
    {
        [DataMember]
        public int PageSize { get; set; } = 15;
        [DataMember]
        public int Page { get; set; } = 1;
    }
}
