using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseName = "playerdb.db3";

            SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + baseName + ";Version=3;");
            cnnect.Open();
            SQLiteCommand command = new SQLiteCommand(); ;
            command.Connection = cnnect;
            command.CommandText = @"INSERT INTO [players]([id], [xcoordinate], [ycoordinate], [size]) "
                        + "VALUES (777, 0, 0, 5);";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
   
        }
    }
}
