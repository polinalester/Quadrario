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
            DataTable dTable = new DataTable();
            string baseName = "playerdb.db3";

            SQLiteConnection cnnect = new SQLiteConnection("Data Source=" + baseName + ";Version=3;");
            cnnect.Open();
            SQLiteCommand command = new SQLiteCommand(); ;
            command.Connection = cnnect;
            command.CommandText = @"SELECT * FROM [players]";
            //command.CommandType = CommandType.Text;
            //command.ExecuteNonQuery();

            try
            {
                SQLiteDataReader r = command.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    line = r["id"].ToString() + ", "
                         + r["xcoordinate"] + ", "
                         + r["ycoordinate"] + ", "
                         + r["size"];
                    Console.WriteLine(line);
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();

        }
    }
}
