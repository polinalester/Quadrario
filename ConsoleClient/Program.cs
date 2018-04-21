using System;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Client {
    internal class Program {
        private static void Main(string[] args) {
            Console.WriteLine("Please insert your ID:");
            var userId = 0;
            while(!Int32.TryParse(Console.ReadLine(), out userId))
            {
                Console.WriteLine("Wrong! Please insert your ID:");
            }
            string baseName = "C:/Users/polina/Downloads/Quadrario/Quadrario/playerdb.db3";

            SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + baseName + ";Version=3;");
            cnnect.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = cnnect;
            command.CommandText = @"INSERT INTO [players]([id], [xcoordinate], [ycoordinate], [size]) "
                        + "VALUES (" + userId + ", 0, 0, 6);";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT * FROM [players]";
           

            try
            {
                SQLiteDataReader r = command.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    line = r["id"].ToString() + ", " + r["xcoordinate"] + ", " + r["ycoordinate"] + ", " + r["size"];
                    Console.WriteLine(line);
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();

            Thread.Sleep(1000);
            Console.WriteLine("Please insert your server IP:");
            string ip = "";
            while(String.IsNullOrWhiteSpace(ip)) {
                ip = Console.ReadLine();
            }
            Console.WriteLine("Type 'exit' to exit...");

            var c = new Client(ip) { Id = userId, IP = ip };


            while (Console.ReadLine() != "exit") {
                Thread.Sleep(10);
            }
            c.Dispose();
        }
    }
}