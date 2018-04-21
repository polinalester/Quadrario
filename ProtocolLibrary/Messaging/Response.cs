using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    [ProtoInclude(2001, typeof (BooleanResponse))]
    [ProtoInclude(2002, typeof(ServiceProviderResponse))]
    public class Response : Message {
        public Response() {
            Type = MessageTypes.Response;
        }
    }
}