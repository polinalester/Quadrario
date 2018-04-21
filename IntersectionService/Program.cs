using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using NetMQ;

namespace IntersectionService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Func<string, string> GetIP = (desc) => {
                while (true)
                {
                    Console.WriteLine("Please provide " + desc + " address (Example: 127.0.0.1)");

                    Regex ipParser = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                    var input = Console.ReadLine();
                    MatchCollection result = ipParser.Matches(input);
                    if (result.Count == 1)
                    {
                        var match = result[0];
                        Console.WriteLine("Got ip " + match.Value);
                        return match.Value;
                    }
                }
            };
            Console.WriteLine("Type 'exit' to exit...");
            var ip = GetIP("This node IP");
            var dirSerIp = GetIP("Directory Service IP");

            var s = new IntersectionService(ip, dirSerIp);
            while (Console.ReadLine() != "exit")
            {
                Thread.Sleep(10);
            }
            s.Dispose();
        }
    }
}
