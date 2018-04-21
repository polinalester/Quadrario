using System;
using ProtoBuf;

//TODO: move into common library!

namespace ProtocolLibrary.Messaging {
    [ProtoContract]
    [ProtoInclude(101, typeof (Request))]
    [ProtoInclude(201, typeof (Response))]
    [ProtoInclude(301, typeof (Event))]
    public class Message {
        [ProtoMember(1)] public MessageTypes Type;

        [ProtoMember(2)] public int UserId;
    }
}