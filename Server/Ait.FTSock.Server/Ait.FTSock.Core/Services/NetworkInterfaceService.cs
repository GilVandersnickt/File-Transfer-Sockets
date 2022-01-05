using Ait.FTSock.Core.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ait.FTSock.Core.Services
{
    public class NetworkInterfaceService
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
        public List<string> GetLocalIpAddresses()
        {
            List<string> localIpAddresses = new List<string>();

            // Loopback address
            localIpAddresses.Add(GetLocalIpv4AddressesByNetworkInterfaceType(NetworkInterfaceType.Loopback).FirstOrDefault());
            // Ip addresses of active NICs
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var address in host.AddressList)
            {
                if (address.AddressFamily.Equals(AddressFamily.InterNetwork))
                    localIpAddresses.Add(address.ToString());
            }

            return localIpAddresses;
        }
        private List<string> GetLocalIpv4AddressesByNetworkInterfaceType(NetworkInterfaceType networkInterfaceType)
        {
            List<string> localIpAddresses = new List<string>();
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType.Equals(networkInterfaceType) && networkInterface.OperationalStatus.Equals(OperationalStatus.Up))
                {
                    foreach (UnicastIPAddressInformation ipAddress in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily.Equals(AddressFamily.InterNetwork))
                        {
                            localIpAddresses.Add(ipAddress.Address.ToString());
                        }
                    }
                }
            }
            return localIpAddresses;
        }
    }
}
