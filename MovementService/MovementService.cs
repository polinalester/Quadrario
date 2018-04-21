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

namespace MovementService
{
    public class MovementService : IDisposable {
        private static volatile bool _stopLoops;
        private string _ip = "";
        private string _dirSerIp = "";

        public MovementService(string ip, string dirSerIp) {
            _ip = ip;
            _dirSerIp = dirSerIp;
            RequestReply();
            PubSub();
            CallDirectoryService();
        }

        public void CallDirectoryService() {
            ThreadPool.QueueUserWorkItem(state => {
                var req = new ServiceHostRequest() { Address = _ip, ServiceType = "move" };
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
                server.Bind("tcp://*:5556");
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
                server.Bind("tcp://*:5555");
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
            switch(r.Type) {
                case MessageTypes.MoveRequest:
                    var mr = (MoveRequest)r;

                    string baseName = "C:/Users/polina/Downloads/Quadrario/Quadrario/playerdb.db3";
              
                    SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + baseName + ";Version=3;");
                    cnnect.Open();
                    SQLiteCommand command = new SQLiteCommand(); ;
                    command.Connection = cnnect;

                    //if(++counter % 30000 == 0) {
                    Console.WriteLine("Move request: {1}; Server: req from {0}", mr.UserId, counter);
                    //}
                    if (mr.Direction[1] == 1)
                    {
                        Console.WriteLine("move right");
                        command.CommandText = @"UPDATE [players] SET [xcoordinate] = [xcoordinate] + 1 "
                            + "WHERE [id] = " + mr.UserId;
                    }
                    else if (mr.Direction[1] == -1)
                    {
                        Console.WriteLine("move left");
                        command.CommandText = @"UPDATE [players] SET [xcoordinate] = [xcoordinate] - 1 "
                            + "WHERE [id] = " + mr.UserId;
                    }
                    else if (mr.Direction[2] == 1)
                    {
                        Console.WriteLine("move up");
                        command.CommandText = @"UPDATE [players] SET [ycoordinate] = [ycoordinate] + 1 "
                            + "WHERE [id] = " + mr.UserId;
                    }
                    else //(mr.Direction[2] == -1)
                    {
                        Console.WriteLine("move down");
                        command.CommandText = @"UPDATE [players] SET [ycoordinate] = [ycoordinate] - 1 "
                            + "WHERE [id] = " + mr.UserId;
                    }
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                    return new BooleanResponse { Ok = true };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}