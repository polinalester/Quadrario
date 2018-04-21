using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ProtoBuf;
using ProtocolLibrary;
using ProtocolLibrary.Datastructures;
using ProtocolLibrary.Messaging;
using NetMQ;
using NetMQ.Sockets;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace RenderService
{
    public class RenderService : IDisposable {
        private static volatile bool _stopLoops;
        private string _ip = "";
        private string _dirSerIp = "";

        public RenderService(string ip, string dirSerIp) {
            _ip = ip;
            _dirSerIp = dirSerIp;
            RequestReply();
            PubSub();
            CallDirectoryService();
        }

        public void CallDirectoryService() {
            ThreadPool.QueueUserWorkItem(state => {
                var req = new ServiceHostRequest() { Address = _ip, ServiceType = "render" };
                var dirServiceSocket = new RequestSocket();
                dirServiceSocket.Connect("tcp://" + _dirSerIp + ":8910");
                while(!_stopLoops) {
                    try {
                        using(var reqStream = new MemoryStream()) {
                            Serializer.Serialize(reqStream, req);
                            dirServiceSocket.SendFrame(reqStream.ToArray());
                        }
                      var receiveFrame = dirServiceSocket.ReceiveFrameBytes();
                            var request = Serializer.Deserialize<Message>(new MemoryStream( receiveFrame)) as BooleanResponse;
                            Console.WriteLine(request.Ok);
                            if(request.Ok) {
                                return;
                            }
                        
                    } catch(Exception e) {
                        Console.WriteLine(e);
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public void Dispose() {
            _stopLoops = true;
        }

        public void PubSub() {
            ThreadPool.QueueUserWorkItem(state => {
                var server = new PublisherSocket();
                server.Bind("tcp://*:5557");
                while(!_stopLoops) {
                    try {
                        var dataFacadeEvent = new DataFacadeEvent { State = new List<User> { new User { Id = 666 } } };
                        using(var responseStream = new MemoryStream()) {
                            Serializer.Serialize(responseStream, dataFacadeEvent);
                            server.SendFrame(responseStream.ToArray());
                        }
                        Thread.Sleep(5000);
                    } catch(Exception e) {
                        Console.WriteLine(e);
                    }
                }
            });
        }

        public void RequestReply() {
            //TODO: move into server app!
            ThreadPool.QueueUserWorkItem(state => {
                var server = new ResponseSocket();
                server.Bind("tcp://*:5558");
                while(!_stopLoops) {
                    try {
                        var receiveFrame = server.ReceiveFrameBytes();
                            var request = Serializer.Deserialize<Request>(new MemoryStream(receiveFrame));
                            var response = OnRequest(request);

                            using(var responseStream = new MemoryStream()) {
                                Serializer.Serialize(responseStream, response);
                                server.SendFrame(responseStream.ToArray());
                            }
                        
                    } catch(Exception e) {
                        Console.WriteLine(e);
                    }
                }
            });
        }

        long counter;

        public Response OnRequest(Request r) {
            var reqtype = r.Type;
            //switch(r.Type) {
              //  case RenderRequest:
                    //var mr = (RenderRequest)r;

                    Console.WriteLine("Done");

                    return new BooleanResponse { Ok = true };
               // default:
                //    throw new ArgumentOutOfRangeException();
            //}
        }
    }
}