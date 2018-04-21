using System.Collections.Generic;
using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    public class MoveRequest : Request {
        public MoveRequest() {
            Type = MessageTypes.MoveRequest;
            Direction = new List<int>();
        }

        [ProtoMember(1)]
        public List<int> Direction { get; set; }
    }

    
    [ProtoContract]
    public class ServiceHostRequest : Request {
        public ServiceHostRequest() {
            Type = MessageTypes.ServiceHostRequest;
        }

        [ProtoMember(1)]
        public string ServiceType;

        [ProtoMember(2)]
        public string Address;
    }

    [ProtoContract]
    public class ServiceProvideRequest : Request {
        public ServiceProvideRequest() {
            Type = MessageTypes.ServiceProviderRequest;
        }

        [ProtoMember(1)]
        public string ServiceType;
    }
}