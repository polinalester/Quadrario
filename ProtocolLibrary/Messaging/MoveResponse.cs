using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    public class BooleanResponse : Response {
        public BooleanResponse() {
            Type = MessageTypes.BooleanResponse;
        }
        [ProtoMember(1)]
        public bool Ok { get; set; }
    }

    [ProtoContract]
    public class ServiceProviderResponse : Response {
        public ServiceProviderResponse() {
            Type = MessageTypes.ServiceProviderResponse;
        }
        [ProtoMember(1)]
        public string Address { get; set; }
    }
}