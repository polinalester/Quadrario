using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace ProtocolLibrary.Messaging
{
    [ProtoContract]
    public class RenderResponse : Response
    {
        public List<int[]> Players; // The nested array I would like to serialize.
        [ProtoMember(1)]
        private List<ProtobufArray<int>> _nestedArrayForProtoBuf // Never used elsewhere
        {
            get
            {
                if (Players == null)  //  ( _nestedArray == null || _nestedArray.Count == 0 )  if the default constructor instanciate it
                    return null;
                return Players.Select(p => new ProtobufArray<int>(p)).ToList();
            }
            set
            {
                Players = value.Select(p => p.MyArray).ToList();
            }
        }



        [ProtoContract]
        public class ProtobufArray<T>   // The intermediate type
        {
            [ProtoMember(1)]
            public T[] MyArray;

            public ProtobufArray()
            { }
            public ProtobufArray(T[] array)
            {
                MyArray = array;
            }
        }
    }
}