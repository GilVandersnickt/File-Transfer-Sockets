using System;

namespace Ait.FTSock.Core.Entities
{
    public class FTFile
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public byte Size { get; set; }
        public DateTime CreationDate { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
