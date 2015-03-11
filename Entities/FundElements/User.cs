#region

using System;

#endregion

namespace IPSCM.Entities
{
    public class User
    {
        public UInt32 UserId { get; protected set; }
        public String PlateNumber { get; protected set; }
        public UInt64 Money { get; protected set; }
        public String PhoneNumber { get; protected set; }
    }
}