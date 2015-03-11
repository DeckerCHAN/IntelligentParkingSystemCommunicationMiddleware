#region

using System;

#endregion

namespace IPSCM.Entities
{
    public class Ticket
    {
        public UInt32 TicketId { get; set; }
        public String Type { get; set; }
        public Int32 Value { get; set; }
        public UInt32 UserId { get; set; }
        public String StoreName { get; set; }
    }
}