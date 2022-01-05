using Ait.FTSock.Core.Constants;
using Ait.FTSock.Core.Enums;
using Newtonsoft.Json;

namespace Ait.FTSock.Core.Services
{
    public class FTService
    {
        public FTService()
        {

        }
        public string GetConnectionRequest(string username)
        {
            string json = JsonConvert.SerializeObject(username);
            return $"{RequestType.CONNECT}{RequestParts.Delimiter}{json}{RequestParts.EndOfMessage}";
        }
        public string GetCloseRequest(string id)
        {
            string json = JsonConvert.SerializeObject(id);
            return $"{RequestType.CLOSE}{RequestParts.Delimiter}{json}{RequestParts.EndOfMessage}";
        }
        public string GetCDUPRequest(string id)
        {
            string json = JsonConvert.SerializeObject(id);
            return $"{RequestType.CDUP}{RequestParts.Delimiter}{json}{RequestParts.EndOfMessage}";
        }
        public string GetCDDIRRequest(string id, string newDirectory)
        {
            string idJson = JsonConvert.SerializeObject(id);
            string newDirectoryJson = JsonConvert.SerializeObject(newDirectory);
            return $"{RequestType.CDDIR}{RequestParts.Delimiter}{idJson}{RequestParts.Delimiter}{newDirectoryJson}{RequestParts.EndOfMessage}";
        }
        public string GetMKDIRRequest(string id, string newDirectory)
        {
            string idJson = JsonConvert.SerializeObject(id);
            string newDirectoryJson = JsonConvert.SerializeObject(newDirectory);
            return $"{RequestType.MKDIR}{RequestParts.Delimiter}{idJson}{RequestParts.Delimiter}{newDirectoryJson}{RequestParts.EndOfMessage}";
        }
        public string GetGETRequest(string id, string fileName)
        {
            string idJson = JsonConvert.SerializeObject(id);
            string fileNameJson = JsonConvert.SerializeObject(fileName);
            return $"{RequestType.GET}{RequestParts.Delimiter}{idJson}{RequestParts.Delimiter}{fileNameJson}{RequestParts.EndOfMessage}";
        }
        public string GetPUTRequest(string id, string fileName, byte[] fileData)
        {
            string idJson = JsonConvert.SerializeObject(id);
            string fileNameJson = JsonConvert.SerializeObject(fileName);
            string fileDataJson = JsonConvert.SerializeObject(fileData);
            return $"{RequestType.PUT}{RequestParts.Delimiter}{idJson}{RequestParts.Delimiter}{fileNameJson}{RequestParts.Delimiter}{fileDataJson}{RequestParts.EndOfMessage}";
        }
    }
}
