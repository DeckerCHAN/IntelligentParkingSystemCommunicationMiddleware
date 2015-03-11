using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPSCM.Entities.Results.Coupon
{
    public class CouponInfo
    {
        public UInt32 TicketId { get; set; }
        public String Type { get; set; }
        public Int32 Value { get; set; }
    }
}
