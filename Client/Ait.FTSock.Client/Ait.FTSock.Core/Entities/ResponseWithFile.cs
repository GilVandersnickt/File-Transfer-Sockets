namespace Ait.FTSock.Core.Entities
{
    public class ResponseWithFile : Response
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
