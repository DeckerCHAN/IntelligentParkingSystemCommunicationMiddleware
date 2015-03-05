using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCM.Configuration;
using Newtonsoft.Json.Linq;

namespace IPSCM.Protocol.Entities.Results
{
    public abstract class Result
    {
        public ResultCode ResultCode { get; protected set; }
        public String ErrorMessage { get; protected set; }
        protected FileConfig JsonConfig { get; private set; }

        protected Result(String jsonString)
        {
            this.JsonConfig = FileConfig.FindConfig("Json.cfg");
            var o = JObject.Parse(jsonString);
            this.ResultCode = (ResultCode)o[this.JsonConfig.GetString("ResultCode")].ToObject<UInt16>();
            this.ErrorMessage = (o.SelectToken(this.JsonConfig.GetString("ErrorMessagePath"), false) ?? String.Empty).ToString();
        }

    }
}
