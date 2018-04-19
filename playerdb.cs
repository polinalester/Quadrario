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

            SQLiteConnection.CreateFile(baseName);

            SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
            using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + baseName;
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"CREATE TABLE [players] (
                    [id] integer PRIMARY KEY NOT NULL,
                    [xcoordinate] integer NOT NULL,
                    [ycoordinate] integer NOT NULL,
                    [size] integer NOT NULL
                    );";
                    
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
