using System.Runtime.Serialization;

namespace MessageWrapperMediatR.Application.Contracts
{
    [DataContract]
    public class HandlerContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsError { get; set; }

        [DataMember]
        public string Queue { get; set; }

        [DataMember]
        public int TimeToLiveInDays { get; set; }

        [DataMember]
        public string AssociateCommand { get; set; }

        [DataMember]
        public bool MessageIsStored { get; set; }

        [DataMember]
        public List<BindingContract> Bindings { get; set; }

        [DataMember]
        public bool IsPermanent { get; set; }

        [DataMember]
        public MessageBusEnumContract BusType { get; set; }
    }
}
