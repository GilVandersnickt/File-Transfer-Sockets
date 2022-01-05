using Ait.FTSock.Core.Constants;
using Ait.FTSock.Core.Entities;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Ait.FTSock.Core.Services
{
    public class ServerService
    {
        public Socket serverSocket;
        public IPEndPoint serverEndpoint;
        public ServerService()
        {

        }
        public string GetResponse(string ipAddress, string port, string request)
        {
            if (serverSocket == null)
            {
                IPAddress serverIP = IPAddress.Parse(ipAddress);
                int serverPort = int.Parse(port);
                serverEndpoint = new IPEndPoint(serverIP, serverPort);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            try
            {
                serverSocket.Connect(serverEndpoint);
                byte[] outMessage = Encoding.ASCII.GetBytes(request);
                byte[] inMessage = new byte[1024];

                serverSocket.Send(outMessage);
                string response = "";
                while (true)
                {
                    int responseLength = serverSocket.Receive(inMessage);
                    response += Encoding.ASCII.GetString(inMessage, 0, responseLength);
                    if (response.IndexOf(RequestParts.EndOfMessage) > -1)
                        break;
                }
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
                serverSocket = null;
                return response;
            }
            catch (Exception error)
            {
                serverSocket = null;
                return "";
            }
        }
        public void Disconnect()
        {
            serverEndpoint = null;
            serverSocket = null;
        }
        public Response ConvertToResponse(string response)
        {
            response = response.Replace(RequestParts.EndOfMessage, "").Trim();
            return JsonConvert.DeserializeObject<Response>(response);
        }
        public ResponseWithFile ConvertToResponseWithFile(string response)
        {
            response = response.Replace(RequestParts.EndOfMessage, "").Trim();
            return JsonConvert.DeserializeObject<ResponseWithFile>(response);
        }

    }
}
