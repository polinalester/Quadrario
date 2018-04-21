using System.Collections.Generic;
using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    public class RenderRequest : Request {
        public RenderRequest() {
            Type = MessageTypes.RenderRequest;
        }

        [ProtoMember(1)]
        public int UID { get; set; }
        
    }

    /*[ProtoContract]
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
            Type = MessageTypes.ServiceProvederRequest;
        }

        [ProtoMember(1)]
        public string ServiceType;
    }*/
}