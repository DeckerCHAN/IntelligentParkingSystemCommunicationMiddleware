using System;
using System.Net;

namespace IPSCM.Protocol.EventArgs
{
    public class ParkingEventArgs : HttpDataEventArgs
    {
        public String PlateNumber { get; private set; }
        public DateTime InTime { get; private set; }
        public Byte[] InImg { get; private set; }
        public ParkingEventArgs(HttpListenerRequest request, HttpListenerResponse response, String plateNumber, DateTime inTime, Byte[] inImg)
            : base(request, response)
        {
            this.PlateNumber = plateNumber;
            this.InTime = inTime;
            this.InImg = inImg;
        }
    }
}
