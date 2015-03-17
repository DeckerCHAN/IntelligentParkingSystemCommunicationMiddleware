#region

using System;

#endregion

namespace IPSCM.Entities.Results
{
    public class Result
    {
        public ResultCode ResultCode { get; set; }
        public String ErrorMsg { get; set; }
    }
}