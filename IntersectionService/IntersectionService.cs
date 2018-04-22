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

namespace IntersectionService
{
    public class IntersectionService : IDisposable
    {
        private static volatile bool _stopLoops;
        private string _ip = "";
        private string _dirSerIp = "";

        public IntersectionService(string ip, string dirSerIp)
        {
            _ip = ip;
            _dirSerIp = dirSerIp;
            RequestReply();
            PubSub();
            CallDirectoryService();
        }

        public void CallDirectoryService()
        {
            ThreadPool.QueueUserWorkItem(state => {
                var req = new ServiceHostRequest() { Address = _ip, ServiceType = "intersect" };
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
                        var request = Serializer.Deserialize<Message>(new MemoryStream(receiveFrame)) as BooleanResponse;
                        Console.WriteLine(request.Ok);
                        if (request.Ok)
                        {
                            return;
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
                var server = new PublisherSocket();
                server.Bind("tcp://*:5554");
                while (!_stopLoops)
                {
                    try
                    {
                        var dataFacadeEvent = new DataFacadeEvent { State = new List<User> { new User { Id = 666 } } };
                        using (var responseStream = new MemoryStream())
                        {
                            Serializer.Serialize(responseStream, dataFacadeEvent);
                            server.SendFrame(responseStream.ToArray());
                        }
                        Console.WriteLine("Sub from user " + dataFacadeEvent.UserId);
                        Thread.Sleep(5000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
        }

        public void RequestReply()
        {
            //TODO: move into server app!
            ThreadPool.QueueUserWorkItem(state => {
                var server = new ResponseSocket();
                server.Bind("tcp://*:5553");
                while (!_stopLoops)
                {
                    try
                    {
                        var receiveFrame = server.ReceiveFrameBytes();
                        var request = Serializer.Deserialize<Request>(new MemoryStream(receiveFrame));
                        var response = OnRequest(request);

                        using (var responseStream = new MemoryStream())
                        {
                            Serializer.Serialize(responseStream, response);
                            server.SendFrame(responseStream.ToArray());
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
        }

        public Response OnRequest(Request r)
        {
           /* switch (r.Type)
            {
                case MessageTypes.Request:*/
                    //var ir = (IntersectRequest)r;
                    var ir = r;
                    string baseName = "C:/Users/polina/Downloads/Quadrario/Quadrario/playerdb.db3";

                    SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + baseName + ";Version=3;");
                    cnnect.Open();
                    SQLiteCommand command = new SQLiteCommand(); ;
                    command.Connection = cnnect;

                    Console.WriteLine("Intersect request from {0}", ir.UserId);
                    int xpos = 0, ypos = 0, size = 0;
                    command.CommandText = @"SELECT * FROM [players] WHERE [id] = " + ir.UserId;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    try
                    {
                        SQLiteDataReader reader = command.ExecuteReader();
                        string line = String.Empty;
                        while (reader.Read())
                        {
                            xpos = Convert.ToInt32(reader["xcoordinate"]);
                            ypos = Convert.ToInt32(reader["ycoordinate"]);
                            size = Convert.ToInt32(reader["size"]);
                        }
                        reader.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    string intersected = "(";
                    command.CommandText = @"SELECT * FROM [players] WHERE [id] != " + ir.UserId
                        + " AND (" + (xpos + size) + " >= [xcoordinate] OR [xcoordinate] + [size] >= "
                        + xpos + " OR [ycoordinate]+[size] >= " + ypos + " OR " + (ypos + size) + " >= [ycoordinate]";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    try
                    {
                        SQLiteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            intersected += reader["id"];
                            intersected += ", ";
                        }
                        if (intersected.Length > 2)
                        {
                            intersected = intersected.Substring(0, intersected.Length - 3);  
                        }   
                        reader.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    intersected += ")";
                    if (intersected.Length > 2)
                    {
                        command.CommandText = @"UPDATE [players] SET [size] = [size] + 10 WHERE [id] = " + ir.UserId;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        command.CommandText = @"UPDATE [players] SET [size] = [size] - 10 WHERE [id] IN (" + intersected;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();

                    }
                    cnnect.Close();
                    return new BooleanResponse { Ok = true };
                /*default:
                    throw new ArgumentOutOfRangeException();
            }*/
        }
    }
}
