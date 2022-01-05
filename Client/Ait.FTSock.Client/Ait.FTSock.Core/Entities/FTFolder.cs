namespace Ait.FTSock.Core.Entities
{
    public class FTFolder
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string Parent { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
