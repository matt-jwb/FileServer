using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileServer
{
    class ClientService
    {
        private Socket Socket;
        private NetworkStream Stream;
        private RemoveClient RemoveFromClientServiceList;
        private ProcessFacade processFacade = ProcessFacade.GetInstance();

        public StreamReader Reader { get; private set; }
        public StreamWriter Writer { get; private set; }

        public ClientService(Socket socket, RemoveClient removeClient)
        {
            Socket = socket;
            RemoveFromClientServiceList = removeClient;
            Stream = new NetworkStream(socket);
            Reader = new StreamReader(Stream, Encoding.UTF8, true);
            Writer = new StreamWriter(Stream, Encoding.UTF8);
            Console.WriteLine($"Client connected from ip {(Socket.RemoteEndPoint as IPEndPoint).Address}");
        }

        public async Task SendResponseAsync()
        {
            try
            {
                string request = await Reader.ReadLineAsync();
                while (request != null && request != "TERMINATE")
                {
                    IResponse response = await ProcessRequestAsync(request);
                    try
                    {
                        await Writer.WriteLineAsync(JsonSerializer.Serialize<ValidResponse>((ValidResponse)response));
                        await Writer.FlushAsync();
                    }
                    catch (Exception)
                    {
                        await Writer.WriteLineAsync(JsonSerializer.Serialize<InvalidResponse>((InvalidResponse)response));
                        await Writer.FlushAsync();
                    }
                    request = await Reader.ReadLineAsync();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }

            Close();
            Console.WriteLine("Closing Connection");
        }

        private async Task<IResponse> ProcessRequestAsync(string request)
        {
            string type;

            try
            {
                var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request);
                type = dictionary["RequestType"].GetString();
            }
            catch (KeyNotFoundException e)
            {
                return new InvalidResponse("ERROR: The request was not correctly formatted");
            }
            catch (Exception e)
            {
                return new InvalidResponse($"ERROR: {e.Message}");
            }

            try
            {
                switch (type)
                {
                    case "RetrieveFile":
                        {
                            RetrieveFileRequest retrieveFileRequest = JsonSerializer.Deserialize<RetrieveFileRequest>(request);
                            return new ValidResponse(await processFacade.RetrieveFile(retrieveFileRequest.FileName));
                        }
                    case "UploadFile":
                        {
                            UploadFileRequest uploadFileRequest = JsonSerializer.Deserialize<UploadFileRequest>(request);
                            return new ValidResponse(await processFacade.UploadFile(uploadFileRequest.FileName, uploadFileRequest.FileData));
                        }
                    case "ListFiles":
                        return new ValidResponse(await processFacade.ListFiles());
                    default:
                        return new InvalidResponse("ERROR: Request not Recognised");
                }
            }
            catch (JsonException)
            {
                return new InvalidResponse($"ERROR: JSON could not be deserialized");
            }
            catch (Exception e)
            {
                return new InvalidResponse($"ERROR: {e.Message}");
            }
        }

        public void Close()
        {
            try
            {
                RemoveFromClientServiceList(this);
                Socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                Console.WriteLine($"Client disconnected from ip {(Socket.RemoteEndPoint as IPEndPoint).Address}");
                Socket.Close();
            }
        }
    }
}
