#region

using System;

#endregion

namespace IPSCM.Entities.Results.Parking
{
    public class ParkingInfo
    {
        public UInt32 RecordId { get; set; }
        public Decimal? Money { get; set; }
        public String PhoneNumber { get; set; }
        public UInt32? UserId { get; set; }
    }
}