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
        GameWindow _gw;

        private string _dirSerIp = "";
        public GameClient(string dirSerIp, GameWindow gw)
        {
            _dirSerIp = dirSerIp;
            _gw = gw;

            CallDirectoryService();
        }


        public int Id { get; set; }

        public string MoveIP = "";
        public string RenderIP = "";
        public string IntersectIP = "";
        public void CallDirectoryService()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var moveReq = new ServiceProvideRequest() { ServiceType = "move" };
                var intersectReq = new ServiceProvideRequest() { ServiceType = "intersect" };
                var renderReq = new ServiceProvideRequest() { ServiceType = "render" };
                var dirServiceSocket = new RequestSocket();
                dirServiceSocket.Connect("tcp://" + _dirSerIp + ":8910");
                while (!_stopLoops)
                {
                    try
                    {
                        using (var reqStream = new MemoryStream())
                        {
                            Serializer.Serialize(reqStream, moveReq);
                            dirServiceSocket.SendFrame(reqStream.ToArray());
                        }
                        var receiveFrame = dirServiceSocket.ReceiveFrameBytes();
                        var request = Serializer.Deserialize<Message>(new MemoryStream(receiveFrame)) as ServiceProviderResponse;
                        //Console.WriteLine(request.Address);
                        if (!String.IsNullOrEmpty(request.Address))
                        {
                            MoveIP = request.Address;
                            PubSub_Move();
                            //RequestReply_Move();
                            //RequestReply();
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    } 
                    try
                    {
                        using (var reqStream = new MemoryStream())
                        {
                            Serializer.Serialize(reqStream, intersectReq);
                            dirServiceSocket.SendFrame(reqStream.ToArray());
                        }
                        var receiveFrame = dirServiceSocket.ReceiveFrameBytes();
                        var request = Serializer.Deserialize<Message>(new MemoryStream(receiveFrame)) as ServiceProviderResponse;
                        if (!String.IsNullOrEmpty(request.Address))
                        {
                            IntersectIP = request.Address;
                            PubSub_Intersect();
                            //RequestReply_Move();
                            //RequestReply();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    try
                    {
                        using (var reqStream = new MemoryStream())
                        {
                            Serializer.Serialize(reqStream, renderReq);
                            dirServiceSocket.SendFrame(reqStream.ToArray());
                        }
                        var receiveFrame = dirServiceSocket.ReceiveFrameBytes();
                        var request = Serializer.Deserialize<Message>(new MemoryStream(receiveFrame)) as ServiceProviderResponse;
                        if (!String.IsNullOrEmpty(request.Address))
                        {
                            RenderIP = request.Address;
                            PubSub_Render();
                            RequestReply_Render();
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
        public void PubSub_Move()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var client = new SubscriberSocket();
                client.Connect("tcp://" + MoveIP + ":5556");
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
        public void PubSub_Intersect()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var client = new SubscriberSocket();
                client.Connect("tcp://" + IntersectIP + ":5554");
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
        public void PubSub_Render()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var client = new SubscriberSocket();
                client.Connect("tcp://" + RenderIP + ":5557");
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
        public void RequestReply_Move(string keyPressed)
        {
            //ThreadPool.QueueUserWorkItem(state =>
            //{
                var client = new RequestSocket();
                client.Connect("tcp://" + MoveIP + ":5555");
                if (keyPressed == "w" || keyPressed == "a" || keyPressed == "s" || keyPressed == "d")
                {
                    try
                    {
                        var request = new MoveRequest { };
                        switch (keyPressed)
                        {
                            case "w":
                            case "W":
                                request = new MoveRequest
                                {
                                    UserId = Id,
                                    Direction = new List<int> { 1, 0, 1 }
                                };
                                break;
                            case "a":
                            case "A":
                                request = new MoveRequest
                                {
                                    UserId = Id,
                                    Direction = new List<int> { 1, -1, 0 }
                                };
                                break;
                            case "s":
                            case "S":
                                request = new MoveRequest
                                {
                                    UserId = Id,
                                    Direction = new List<int> { 1, 0, -1 }
                                };
                                break;
                            case "d":
                            case "D":
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
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            //});
        }
        public void RequestReply_Intersect()
        {
            //ThreadPool.QueueUserWorkItem(state =>
            //{
                var client = new RequestSocket();
                client.Connect("tcp://" + IntersectIP + ":5553");
                try
                {
                    var request = new Request() { UserId = Id };
                    using (var responseStream = new MemoryStream())
                    {
                        Serializer.Serialize(responseStream, request);
                        client.SendFrame(responseStream.ToArray());
                    }
                    var receiveFrame = client.ReceiveFrameBytes();
                    var reply = Serializer.Deserialize<Response>(new MemoryStream(receiveFrame));
                    var mr = reply as BooleanResponse;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            //});
        }
        public void RequestReply_Render()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                List<int[]> players = new List<int[]>();
                var client = new RequestSocket();
                client.Connect("tcp://" + RenderIP + ":5558");

                try
                {
                    var request = new Request() { UserId = Id };
                    using (var responseStream = new MemoryStream())
                    {
                        Serializer.Serialize(responseStream, request);
                        client.SendFrame(responseStream.ToArray());
                    }
                    var receiveFrame = client.ReceiveFrameBytes();
                    var reply = Serializer.Deserialize<RenderResponse>(new MemoryStream(receiveFrame));
                    var mr = reply as RenderResponse; //TODO: Change to render response
                    for (int i = 0; i < mr.Players.Count; i++)
                        players.Add(mr.Players[i]);
                    _gw.players.Clear();
                    for (int i = 0; i < players.Count; i++)
                        _gw.players.Add(players[i]);
                    _gw.RefreshGame();
                    _gw.DrawPlayers();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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
