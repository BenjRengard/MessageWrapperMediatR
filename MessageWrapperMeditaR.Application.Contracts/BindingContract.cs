using System.Runtime.Serialization;

namespace MessageWrapperMediatR.Application.Contracts
{
    [DataContract]
    public class BindingContract
    {
        [DataMember]
        public string Exchange { get; set; }

        [DataMember]
        public string RoutingKey { get; set; }
    }
}
