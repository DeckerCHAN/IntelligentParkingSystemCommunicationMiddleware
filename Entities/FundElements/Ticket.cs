using System;

namespace IPSCM.Entities
{
    public class Ticket
    {
        public UInt32 TicketId { get; protected set; }
        public String Type { get; protected set; }
        public Int32 Value { get; protected set; }
        public UInt32 UserId { get; protected set; }
        public String StoreName { get; protected set; }
    }
}
