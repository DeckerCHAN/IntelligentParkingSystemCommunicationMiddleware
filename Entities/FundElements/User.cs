using System;

namespace IPSCM.Entities
{
    class User
    {
        public UInt32 UserId { get; protected set; }
        public String PlateNumber { get; protected set; }
        public UInt64 Money { get; protected set; }
        public String PhoneNumber { get; protected set; }
    }
}
