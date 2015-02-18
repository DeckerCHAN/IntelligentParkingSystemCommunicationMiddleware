using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace IPSCM.Utils
{
    public static class NetWorkUtils
    {
        public static Boolean IsAvailablePortNumber(UInt32 port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            return tcpConnInfoArray.All(tcpi => tcpi.LocalEndPoint.Port != port);
        }
    }
}
