using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IPSCM.Protocol.EventArgs
{
    public class ParkingEventArgs : HttpDataEventArgs
    {
        public UInt64 RecordId { get; private set; }
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
