#region

using System;
using System.Net;

#endregion

namespace IPSCM.Protocol.EventArgs
{
    public class LeavingEventArgs : HttpDataEventArgs
    {
        public LeavingEventArgs(HttpListenerRequest request, HttpListenerResponse response, String plateNumber,
            DateTime outTime, Byte[] outImg, Decimal copeMoney, Decimal actualMoney, UInt32 ticketId)
            : base(request, response)
        {
            this.PlateNumber = plateNumber;
            this.OutTime = outTime;
            this.OutImg = outImg;
            this.copeMoney = copeMoney;
            this.actualMoney = actualMoney;
            this.TicketId = ticketId;
        }

        public String PlateNumber { get; private set; }
        public DateTime OutTime { get; private set; }
        public Byte[] OutImg { get; private set; }
        public Decimal copeMoney { get; private set; }
        public Decimal actualMoney { get; private set; }
        public UInt32 TicketId { get; private set; }
    }
}