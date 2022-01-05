using System;

namespace Ait.FTSock.Core.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string ActivePath { get; set; }
    }
}
