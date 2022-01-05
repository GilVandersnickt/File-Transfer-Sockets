namespace Ait.FTSock.Core.Services
{
    public class ValidatorService
    {
        public bool Validate(object ipAddress, object port)
        {
            return ValidateIpAddress(ipAddress) && ValidatePort(port);
        }
        private static bool ValidateIpAddress(object ipAddress)
        {
            return ipAddress != null;
        }
        private static bool ValidatePort(object port)
        {
            return port != null;
        }
    }
}
