#region

using System;
using System.Net;

#endregion

namespace IPSCM.Protocol.EventArgs
{
    public class CouponEventArgs : HttpDataEventArgs
    {
        public CouponEventArgs(HttpListenerRequest request, HttpListenerResponse response, String plateNumber)
            : base(request, response)
        {
            this.PlateNumber = plateNumber;
        }

        public String PlateNumber { get; private set; }
    }
}