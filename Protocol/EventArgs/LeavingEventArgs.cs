using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IPSCM.Protocol.EventArgs
{
    public class LeavingEventArgs : HttpDataEventArgs
    {
        public String PlateNumber { get; private set; }
        public DateTime OutTime { get; private set; }
        public Byte[] OutImg { get; private set; }
        public UInt32 copeMoney { get; private set; }
        public UInt32 actualMoney { get; private set; }
        public UInt64 TicketId { get; private set; }

        public LeavingEventArgs(HttpListenerRequest request, HttpListenerResponse response, String plateNumber, DateTime outTime, Byte[] outImg, UInt32 copeMoney, UInt32 actualMoney, UInt64 ticketId)
            : base(request, response)
        {
            this.PlateNumber = plateNumber;
            this.OutTime = outTime;
            this.OutImg = outImg;
            this.copeMoney = copeMoney;
            this.actualMoney = actualMoney;
            this.TicketId = ticketId;
        }
    }
}
