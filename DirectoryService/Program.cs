using System;
using System.Threading;
using NetMQ;
using DirectoryService;


namespace DirectoryService {
    internal class Program {
        private static void Main(string[] args) {
            var s = new DirectoryService();
            while(Console.ReadLine() != "exit") {
                Thread.Sleep(10);
            }
            s.Dispose();
        }
    }
}