using Ait.FTSock.Core.Constants;
using Ait.FTSock.Core.Entities;
using Ait.FTSock.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Ait.FTSock.Core.Services
{
    public class ServerService
    {
        private FTService ftService;
        public Socket ServerSocket;
        public IPEndPoint ServerEndpoint;
        public bool ServerOnline = false;
        public string BasePath;
        public ClientService clientService;
        public List<string> CommunicationStrings { get; set; }

        public ServerService()
        {
            ftService = new FTService();
            CommunicationStrings = new List<string>();
            clientService = new ClientService();
        }

        public void Start(string address, int port, string basePath)
        {
            ServerOnline = true;
            this.BasePath = basePath;
            IPAddress ipAddress = IPAddress.Parse(address);
            ServerEndpoint = new IPEndPoint(ipAddress, port);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Stop()
        {
            ServerOnline = false;
            ServerSocket.Close();
            ServerSocket = null;
            ServerEndpoint = null;
            CommunicationStrings = new List<string>();
        }
        public void HandleClientCall(Socket clientCall)
        {
            byte[] clientRequest = new Byte[1024];
            string instruction = null;

            while (true)
            {
                int numByte = clientCall.Receive(clientRequest);
                instruction += Encoding.ASCII.GetString(clientRequest, 0, numByte);
                if (instruction.IndexOf(CommunicationParts.EndOfMessage) > -1)
                    break;
            }
            string serverResponseInText = ProcessClientCall(instruction);
            if (serverResponseInText != "")
            {
                byte[] serverResponse = Encoding.ASCII.GetBytes(serverResponseInText);
                clientCall.Send(serverResponse);
            }
            clientCall.Shutdown(SocketShutdown.Both);
            clientCall.Close();
        }

        public string ProcessClientCall(string instruction)
        {
            instruction = instruction.Replace(CommunicationParts.EndOfMessage, "").Trim();
            // CONNECT
            if (instruction.Length > RequestType.CONNECT.ToString().Length && instruction.Substring(0, RequestType.CONNECT.ToString().Length) == RequestType.CONNECT.ToString())
            {
                Response response = ftService.GetCONNECTResponse(instruction, BasePath);
                clientService.AddClient(response.Username, response.Id, response.ActivePath);
                AddCommunication(response, RequestType.CONNECT);
                return ftService.ConvertToResponseString(response);
            }
            // CLOSE
            else if (instruction.Length > RequestType.CLOSE.ToString().Length && instruction.Substring(0, RequestType.CLOSE.ToString().Length).Equals(RequestType.CLOSE.ToString()))
            {
                Response response = ftService.GetCLOSEResponse(instruction);
                clientService.RemoveClientById(response.Id);
                AddCommunication(response, RequestType.CLOSE);
                return ftService.ConvertToResponseString(response.Id);
            }
            // CDUP
            else if (instruction.Length > RequestType.CDUP.ToString().Length && instruction.Substring(0, RequestType.CDUP.ToString().Length).Equals(RequestType.CDUP.ToString()))
            {
                Client client = clientService.GetClientById(ftService.GetRequestId(instruction));
                Response response = ftService.GetCDUPResponse(BasePath, client);
                clientService.UpdateClient(client, response.ActivePath);
                AddCommunication(response, RequestType.CDUP);
                return ftService.ConvertToResponseString(response);
            }
            // CDDIR
            else if (instruction.Length > RequestType.CDDIR.ToString().Length && instruction.Substring(0, RequestType.CDDIR.ToString().Length).Equals(RequestType.CDDIR.ToString()))
            {
                Client client = clientService.GetClientById(ftService.GetRequestId(instruction));
                Response response = ftService.GetCDDIRResponse(instruction, client);
                clientService.UpdateClient(client, response.ActivePath);
                AddCommunication(response, RequestType.CDDIR);
                return ftService.ConvertToResponseString(response);
            }
            // MKDIR
            else if (instruction.Length > RequestType.MKDIR.ToString().Length && instruction.Substring(0, RequestType.MKDIR.ToString().Length).Equals(RequestType.MKDIR.ToString()))
            {
                Client client = clientService.GetClientById(ftService.GetRequestId(instruction));
                Response response = ftService.GetMKDIRResponse(instruction, client);
                clientService.UpdateClient(client, response.ActivePath);
                AddCommunication(response, RequestType.MKDIR);
                return ftService.ConvertToResponseString(response);
            }
            // GET
            else if (instruction.Length > RequestType.GET.ToString().Length && instruction.Substring(0, RequestType.GET.ToString().Length).Equals(RequestType.GET.ToString()))
            {
                Client client = clientService.GetClientById(ftService.GetRequestId(instruction));
                ResponseWithFile response = ftService.GetGETResponse(instruction, client);
                clientService.UpdateClient(client, response.ActivePath);
                AddCommunication(response, RequestType.GET);
                return ftService.ConvertToResponseString(response);
            }
            // PUT
            else if (instruction.Length > RequestType.PUT.ToString().Length && instruction.Substring(0, RequestType.PUT.ToString().Length).Equals(RequestType.PUT.ToString()))
            {
                Client client = clientService.GetClientById(ftService.GetRequestId(instruction));
                ResponseWithFile responseWithFile = ftService.GetPUTResponse(instruction, client);
                Response response = new Response { Username = responseWithFile.Username, Id = responseWithFile.Id, ActivePath = responseWithFile.ActivePath, Files = responseWithFile.Files, SubFolders = responseWithFile.SubFolders };
                clientService.UpdateClient(client, response.ActivePath);
                AddCommunication(responseWithFile, RequestType.PUT);
                return ftService.ConvertToResponseString(response);
            }
            else return $"{CommunicationParts.Error}{CommunicationParts.EndOfMessage}";

        }
        public void AddCommunication(Response response, RequestType requestType)
        {
            if (requestType.Equals(RequestType.CONNECT))
                CommunicationStrings.Add($"{response.Username} (id={response.Id}) connected. AP = {response.ActivePath}");
            else if (requestType.Equals(RequestType.CLOSE))
                CommunicationStrings.Add($"(id={response.Id}) disconnected.");
            else if (requestType.Equals(RequestType.CDUP))
                CommunicationStrings.Add($"{response.Username} (id={response.Id} requested {requestType}. AP = {response.ActivePath}");
            else if (requestType.Equals(RequestType.CDDIR))
                CommunicationStrings.Add($"{response.Username} (id={response.Id} requested {requestType} {response.ActivePath.Split('\\').Last()}. AP = {response.ActivePath}");
            else if (requestType.Equals(RequestType.MKDIR))
                CommunicationStrings.Add($"{response.Username} (id={response.Id} requested {requestType} {response.ActivePath.Split('\\').Last()}. AP = {response.ActivePath}");

        }
        public void AddCommunication(ResponseWithFile response, RequestType requestType)
        {
            if (requestType.Equals(RequestType.GET))
                CommunicationStrings.Add($"{response.Username} (id={response.Id} requested {requestType} {response.FileName}. AP = {response.ActivePath}");
            else if (requestType.Equals(RequestType.PUT))
                CommunicationStrings.Add($"{response.Username} (id={response.Id} requested {requestType} {response.FileName}. AP = {response.ActivePath}");

        }
    }
}
