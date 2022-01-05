using Ait.FTSock.Core.Constants;
using Ait.FTSock.Core.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ait.FTSock.Core.Services
{
    public class FTService
    {
        public List<string> CommunicationStrings { get; set; }
        public FTService()
        {
            CommunicationStrings = new List<string>();
        }
        public List<FTFolder> GetFTFolders(string activePath)
        {
            List<FTFolder> folders = new List<FTFolder>();
            foreach (var folderPath in Directory.EnumerateDirectories(activePath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                folders.Add(new FTFolder
                {
                    Name = directoryInfo.Name,
                    FullPath = directoryInfo.FullName,
                    Parent = directoryInfo.Parent.FullName
                });
            }
            return folders;
        }
        public List<FTFile> GetFTFiles(string activePath)
        {
            List<FTFile> files = new List<FTFile>();
            foreach (var filePath in Directory.EnumerateFiles(activePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                files.Add(new FTFile
                {
                    Name = fileInfo.Name,
                    FullPath = fileInfo.FullName,
                    Size = (byte)fileInfo.Length,
                    CreationDate = fileInfo.CreationTime.Date
                });
            }
            return files;
        }
        public Response GetCONNECTResponse(string instruction, string activePath)
        {
            string[] parts = instruction.Split(CommunicationParts.Delimiter);
            string username = JsonConvert.DeserializeObject<string>(parts[1]);
            Guid id = Guid.NewGuid();

            Response response = new Response
            {
                Username = username,
                Id = id,
                ActivePath = activePath,
                SubFolders = GetFTFolders(activePath),
                Files = GetFTFiles(activePath),
            };
            return response;
        }
        public Response GetCLOSEResponse(string instruction)
        {
            string[] parts = instruction.Split(CommunicationParts.Delimiter);
            Guid id = JsonConvert.DeserializeObject<Guid>(parts[1]);

            Response response = new Response { Id = id };
            return response;
        }
        public Response GetCDUPResponse(string serverBasePath, Client client)
        {
            string newDirectory = "";
            if (client.ActivePath.Equals(serverBasePath))
                newDirectory = client.ActivePath;
            else
                newDirectory = Directory.GetParent(client.ActivePath).FullName;

            Response response = new Response
            {
                Username = client.Username,
                Id = client.Id,
                ActivePath = newDirectory,
                SubFolders = GetFTFolders(newDirectory),
                Files = GetFTFiles(newDirectory)
            };
            return response;
        }
        public Response GetCDDIRResponse(string instruction, Client client)
        {
            string[] parts = instruction.Split(CommunicationParts.Delimiter);
            string newDirectory = JsonConvert.DeserializeObject<string>(parts[2]);

            Response response = new Response
            {
                Username = client.Username,
                Id = client.Id,
                ActivePath = newDirectory,
                SubFolders = GetFTFolders(newDirectory),
                Files = GetFTFiles(newDirectory)
            };
            return response;
        }
        public Response GetMKDIRResponse(string instruction, Client client)
        {
            string[] parts = instruction.Split(CommunicationParts.Delimiter);

            string newDirectory = Path.Combine(client.ActivePath, JsonConvert.DeserializeObject<string>(parts[2]));
            Directory.CreateDirectory(newDirectory);

            Response response = new Response
            {
                Username = client.Username,
                Id = client.Id,
                ActivePath = newDirectory,
                SubFolders = GetFTFolders(newDirectory),
                Files = GetFTFiles(newDirectory)
            };
            return response;
        }
        public ResponseWithFile GetGETResponse(string instruction, Client client)
        {
            string[] parts = instruction.Split(CommunicationParts.Delimiter);
            string fileName = JsonConvert.DeserializeObject<string>(parts[2]);
            DirectoryInfo directory = new DirectoryInfo(client.ActivePath);
            byte[] fileData = File.ReadAllBytes(Path.Combine(directory.FullName, fileName));

            ResponseWithFile response = new ResponseWithFile
            {
                Username = client.Username,
                Id = client.Id,
                ActivePath = client.ActivePath,
                SubFolders = GetFTFolders(client.ActivePath),
                Files = GetFTFiles(client.ActivePath),
                FileName = fileName,
                FileBytes = fileData
            };
            return response;
        }
        public ResponseWithFile GetPUTResponse(string instruction, Client client)
        {
            string[] parts = instruction.Split(CommunicationParts.Delimiter);
            string fileName = JsonConvert.DeserializeObject<string>(parts[2]);
            byte[] bytes = JsonConvert.DeserializeObject<byte[]>(parts[3]);

            BinaryWriter binaryWriter = new BinaryWriter(File.Open(Path.Combine(client.ActivePath, fileName), FileMode.Append));
            binaryWriter.Write(bytes, 0, bytes.Length);
            binaryWriter.Close();

            ResponseWithFile response = new ResponseWithFile
            {
                Username = client.Username,
                Id = client.Id,
                ActivePath = client.ActivePath,
                SubFolders = GetFTFolders(client.ActivePath),
                Files = GetFTFiles(client.ActivePath),
                FileName = fileName,
                FileBytes = bytes
            };
            return response;
        }
        public string ConvertToResponseString(Response response)
        {
            return JsonConvert.SerializeObject(response) + CommunicationParts.EndOfMessage;
        }
        public string ConvertToResponseString(Guid userId)
        {
            return JsonConvert.SerializeObject(userId) + CommunicationParts.EndOfMessage;
        }
        public Guid GetRequestId(string instruction)
        {
            string[] parts = instruction.Split(CommunicationParts.Delimiter);
            return JsonConvert.DeserializeObject<Guid>(parts[1]);

        }
    }
}
