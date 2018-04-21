using ProtoBuf;

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    [ProtoInclude(3001, typeof (DataFacadeEvent))]
    public class Event : Message {
        public Event() {
            Type = MessageTypes.Event;
        }
    }
}