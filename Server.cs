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
            Console.WriteLine($"Listening for TCP connection requests on {tcpListener.LocalEndpoint}:{port}");
            tcpListener.Start();
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
    }
}
