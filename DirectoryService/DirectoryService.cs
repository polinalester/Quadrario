using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ProtoBuf;
using ProtocolLibrary;
using ProtocolLibrary.Datastructures;
using ProtocolLibrary.Messaging;
using NetMQ;
using System.Collections.Concurrent;
using NetMQ.Sockets;

namespace DirectoryService {
    public class DirectoryService {
        private static volatile bool _stopLoops;

        public DirectoryService() {
            RequestReply();
        }

        public void Dispose() {
            _stopLoops = true;
        }

        public void RequestReply() {
            //TODO: move into server app!
            ThreadPool.QueueUserWorkItem(state => {
                var server = new ResponseSocket();
                server.Bind("tcp://*:8910");
                while (!_stopLoops) {
                    try {
                        var receiveFrame = server.ReceiveFrameBytes();
                            var request = Serializer.Deserialize<Message>(new MemoryStream(receiveFrame));
                            var response = OnRequest(request);

                            using (var responseStream = new MemoryStream()) {
                                Serializer.Serialize(responseStream, response);
                                server.SendFrame(responseStream.ToArray());
                            }
                        
                    }
                    catch (Exception e) {
                        Console.WriteLine(e);
                    }
                }
            });
        }

        long counter;

        ConcurrentDictionary<string, ConcurrentBag<string>> Services = new ConcurrentDictionary<string,ConcurrentBag<string>>();

        public Response OnRequest(Message r) {
            var response = new BooleanResponse { Ok = false }; // Figure out exception mechanics
            switch (r.Type) {
                case MessageTypes.ServiceHostRequest:
                    var m = r as ServiceHostRequest;
                    ConcurrentBag<string> existing = new ConcurrentBag<string> { m.Address };

                    var list = Services.GetOrAdd(m.ServiceType, existing);
                    if(list != existing) {
                        list.Add(m.Address);
                    }
                    response.Ok = true;
                    return response;
                case MessageTypes.ServiceProvederRequest:
                    var req = r as ServiceProvideRequest;
                    var resp= new ServiceProvederResponse();
                    ConcurrentBag<string> providers = null;
                    if(Services.TryGetValue(req.ServiceType, out providers)) {
                        string s = "";
                        providers.TryPeek(out s); // TODO: Take a random item
                        if(!string.IsNullOrEmpty(s)) {
                            resp.Address = s;
                            return resp;
                        }
                    }
                    return response;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
