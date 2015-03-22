using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IPSCM.Protocol.EventArgs
{
    public class ImageUpdateEventArgs : HttpDataEventArgs
    {
        public String PlateNumber { get; private set; }
        public String Time { get; private set; }
        public String Type { get; private set; }
        public Byte[] Image { get; private set; }

        public ImageUpdateEventArgs(HttpListenerRequest request, HttpListenerResponse response, String plateNumber, String time, Byte[] image, String type)
            : base(request, response)
        {
            this.PlateNumber = plateNumber;
            this.Time = time;
            this.Type = type;
            this.Image = image;
        }
    }
}
