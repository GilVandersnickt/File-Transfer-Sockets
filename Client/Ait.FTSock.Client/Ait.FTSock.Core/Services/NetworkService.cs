using Ait.FTSock.Core.Constants;
using System.Collections.Generic;

namespace Ait.FTSock.Core.Services
{
    public class NetworkService
    {
        public List<int> GetPorts()
        {
            List<int> ports = new List<int>();
            for (int i = PortValues.MinValue; i < (PortValues.MaxValue + 1); i++)
            {
                ports.Add(i);
            }
            return ports;
        }
    }
}
