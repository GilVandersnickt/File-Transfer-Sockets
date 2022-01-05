using System;
using System.Collections.Generic;

namespace Ait.FTSock.Core.Entities
{
    public class Response
    {
        public string Username { get; set; }
        public string ActivePath { get; set; }
        public Guid Id { get; set; }
        public List<FTFolder> SubFolders { get; set; }
        public List<FTFile> Files { get; set; }
    }
}
