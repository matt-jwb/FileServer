using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    delegate void RemoveClient(ClientService clientService);

    class Server
    {
        private TcpListener tcpListener;
        private List<ClientService> clientServices;
        private readonly object clientServicesLock = new object();
        private int port = 4444;

        public Server()
        {
            IPAddress iPAddress = IPAddress.Any;
            tcpListener = new TcpListener(iPAddress, port);
            clientServices = new List<ClientService>();
        }

        private void RemoveClient(ClientService clientService)
        {
            lock (clientServicesLock)
            {
                clientServices.Remove(clientService);
            }
        }

        public void Start()
        {
            // Creates a folder to use if it does not already exist
            Directory.CreateDirectory(@"C:\MattFileSystem\ServerStorage");

            //Listens for tcp connections
            tcpListener.Start();
            Console.WriteLine($"Listening for TCP connection requests on {GetIPAddress()}:{port}");
            while (true)
            {
                Socket socket = tcpListener.AcceptSocket();
                ClientService clientService = new ClientService(socket, RemoveClient);
                clientServices.Add(clientService);
                Task.Run(clientService.SendResponseAsync);
            }
        }

        public void Stop()
        {
            tcpListener.Stop();
        }

        private static string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (var ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.ToString().StartsWith("127."))
                {
                    return ip.ToString();
                }
            }

            return "0.0.0.0";
        }
    }
}
