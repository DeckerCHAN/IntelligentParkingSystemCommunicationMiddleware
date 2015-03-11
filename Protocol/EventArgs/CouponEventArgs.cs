using System;
using System.Net;

namespace IPSCM.Protocol.EventArgs
{
    public class CouponEventArgs : HttpDataEventArgs
    {
        public String PlateNumber { get; private set; }

        public CouponEventArgs(HttpListenerRequest request, HttpListenerResponse response, String plateNumber)
            : base(request, response)
        {
            this.PlateNumber = plateNumber;
        }
    }
}