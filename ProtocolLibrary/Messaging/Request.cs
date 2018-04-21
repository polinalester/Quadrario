using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    [ProtoInclude(1001, typeof(MoveRequest))]
    [ProtoInclude(1002, typeof(ServiceHostRequest))]
    [ProtoInclude(1003, typeof(ServiceProvideRequest))]
    public class Request : Message {
        public Request() {
            Type = MessageTypes.Request;
        }
    }
}