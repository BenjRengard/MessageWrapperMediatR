using System.Runtime.Serialization;

namespace MessageWrapperMediatR.Application.Contracts
{
    [DataContract]
    public class PaginatedResponseContract<T>
    {
        [DataMember]
        public List<T> Items { get; set; }

        [DataMember]
        public int TotalItems { get; set; }
    }
}
