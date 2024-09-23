using System;

namespace FileServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Matt Bonham's File Server V1 started");
            Server server = new Server();
            server.Start();
            server.Stop();
            Console.WriteLine("Matt Bonham's File Server V1 stopped");
        }
    }
}
