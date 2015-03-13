#region

using System;

#endregion

namespace IPSCM.Entities.FundElements
{
    public class User
    {
        public UInt32 UserId { get; set; }
        public String PlateNumber { get; set; }
        public Decimal Money { get; set; }
        public String PhoneNumber { get; set; }
    }
}