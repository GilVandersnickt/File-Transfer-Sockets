using System;
using System.Linq;

namespace Ait.FTSock.Core.Services
{
    public class ValidatorService
    {
        public bool Validate(string ipAddress, object port)
        {
            return ValidateIpAddress(ipAddress) && ValidatePort(port);
        }

        private static bool ValidateIpAddress(string ipAddress)
        {
            if (String.IsNullOrWhiteSpace(ipAddress))
                return false;

            string[] splitAddresses = ipAddress.Split('.');
            if (splitAddresses.Length != 4)
                return false;

            return splitAddresses.All(splitAddress => byte.TryParse(splitAddress, out byte result));
        }
        private static bool ValidatePort(object port)
        {
            return port != null;
        }
    }
}
