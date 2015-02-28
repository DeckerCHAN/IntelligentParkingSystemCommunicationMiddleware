using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPSCM.Protocol.Entities
{
    public enum ResultCode
    {
        Success = 200,
        SuccessButNoBinding = 210,
        SuccessButInsufficientFunds = 220,
        WrongArguments = 400,
        WrongToken = 410,
        WrongSign = 411,
        ServerFailure = 500

        

    }
}
