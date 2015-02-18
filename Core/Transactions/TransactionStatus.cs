using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPSCM.Core.Transactions
{
    public enum TransactionStatus
    {
        Immature = 1,
        Started = 2,
        Exhausted = 3,
        Errored = -1
    }
}
