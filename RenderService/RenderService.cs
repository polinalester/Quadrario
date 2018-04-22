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
        string dbName = "C:/Users/polina/Downloads/Quadrario/Quadrario/playerdb.db3";
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
                    Thread.Sleep(900);
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
        public Response OnRequest(Request rr) {
            List<int[]> players = new List<int[]>();
            SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + dbName + ";Version=3;");
            cnnect.Open();
            SQLiteCommand command = new SQLiteCommand(); ;
            command.Connection = cnnect;
            command.CommandText = @"SELECT * FROM [players]";
            Console.WriteLine("Render request from {0}", rr.UserId);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            try
            {
                SQLiteDataReader reader = command.ExecuteReader();
                string line = String.Empty;
                while (reader.Read())
                {
                    int[] playerInfo = new int[4];
                    playerInfo[0] = Convert.ToInt32(reader["id"]);
                    playerInfo[1] = Convert.ToInt32(reader["xcoordinate"]);
                    playerInfo[2] = Convert.ToInt32(reader["ycoordinate"]);
                    playerInfo[3] = Convert.ToInt32(reader["size"]);
                    players.Add(playerInfo);
                }
                reader.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            cnnect.Close();
            return new RenderResponse() { Players = players };
        }
    }
}