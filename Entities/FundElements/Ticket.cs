#region

using System;

#endregion

namespace IPSCM.Entities.FundElements
{
    public class Ticket
    {
        public UInt32 TicketId { get; set; }
        public String Type { get; set; }
        public Decimal Value { get; set; }
        public UInt32 UserId { get; set; }
        public String StoreName { get; set; }
    }
}