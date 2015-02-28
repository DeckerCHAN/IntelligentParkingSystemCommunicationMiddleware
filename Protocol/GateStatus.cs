using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPSCM.Protocol
{
    public enum GateStatus
    {
        WaitingInitialization = 0,
        Initialized = 1,
        Started = 2,
        Endded = 4
    }
}
