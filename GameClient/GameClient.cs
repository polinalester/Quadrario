using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ProtocolLibrary;
using ProtocolLibrary.Messaging;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.IO;

namespace GameClient
{
    internal class GameClient : IDisposable
    {
        private static volatile bool _stopLoops;
        private readonly Random _random = new Random();

        private string _dirSerIp = "";
        public GameClient(string dirSerIp)
        {
            _dirSerIp = dirSerIp;


            CallDirectoryService();
        }


        public int Id { get; set; }

        public void CallDirectoryService()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var req = new ServiceProvideRequest() { ServiceType = "move" };
                var dirServiceSocket = new RequestSocket();
                dirServiceSocket.Connect("tcp://" + _dirSerIp + ":8910");
                while (!_stopLoops)
                {
                    try
                    {
                        using (var reqStream = new MemoryStream())
                        {
                            Serializer.Serialize(reqStream, req);
                            dirServiceSocket.SendFrame(reqStream.ToArray());
                        }
                        var receiveFrame = dirServiceSocket.ReceiveFrameBytes();
                        var request = Serializer.Deserialize<Message>(new MemoryStream(receiveFrame)) as ServiceProviderResponse;
                        //Console.WriteLine(request.Address);
                        if (!String.IsNullOrEmpty(request.Address))
                        {
                            IP = request.Address;
                            PubSub();
                            //RequestReply();
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public void Dispose()
        {
            _stopLoops = true;
        }

        public void PubSub()
        {
            ThreadPool.QueueUserWorkItem(state => {
                var client = new SubscriberSocket();
                client.Connect("tcp://" + IP + ":5556");
                while (!_stopLoops)
                {
                    try
                    {
                        var receiveFrame = client.ReceiveFrameBytes();
                        var @event = Serializer.Deserialize<Event>(new MemoryStream(receiveFrame));

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
        }

        private long counter;

        public void RequestReply(string keyPressed)
        {
            ThreadPool.QueueUserWorkItem(state => {
                var client = new RequestSocket();
                client.Connect("tcp://" + IP + ":5555");
                /*while (!_stopLoops)
                {*/
                    //var key = Console.ReadLine();
                    if (keyPressed == "w" || keyPressed == "a" || keyPressed == "s" || keyPressed == "d")
                    {
                        try
                        {
                            var request = new MoveRequest { };
                            switch (keyPressed)
                            {
                                case "w":
                                    request = new MoveRequest
                                    {
                                        UserId = Id,
                                        Direction = new List<int> { 1, 0, 1 }
                                    };
                                    break;
                                case "a":
                                    request = new MoveRequest
                                    {
                                        UserId = Id,
                                        Direction = new List<int> { 1, -1, 0 }
                                    };
                                    break;
                                case "s":
                                    request = new MoveRequest
                                    {
                                        UserId = Id,
                                        Direction = new List<int> { 1, 0, -1 }
                                    };
                                    break;
                                case "d":
                                    request = new MoveRequest
                                    {
                                        UserId = Id,
                                        Direction = new List<int> { 1, 1, 0 }
                                    };
                                    break;
                                default:
                                    break;

                            }

                            using (var responseStream = new MemoryStream())
                            {
                                Serializer.Serialize(responseStream, request);
                                client.SendFrame(responseStream.ToArray());
                            }
                            var receiveFrame = client.ReceiveFrameBytes();
                            var reply = Serializer.Deserialize<Response>(new MemoryStream(receiveFrame));
                            var mr = reply as BooleanResponse;
          
                            /*if (++counter % 30000 == 0)
                            {
                                Console.WriteLine("response {1}; Client: {0}", mr != null && mr.Ok, counter);
                            }*/

                        }

                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
               // }*/
            });
        }

        public void OnEvent(Event e)
        {
            var dfe = e as DataFacadeEvent;
            if (dfe != null) dfe.State.ForEach(user => Console.WriteLine(user.Id));
        }

        public string IP { get; set; }
    }
}
