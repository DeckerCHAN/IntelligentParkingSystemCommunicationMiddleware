using System;
using Newtonsoft.Json;

namespace IPSCM.Entities.Results
{
    public class Result
    {
        public ResultCode ResultCode { get;  set; }
        public String ErrorMessage { get;  set; }

    }
}
