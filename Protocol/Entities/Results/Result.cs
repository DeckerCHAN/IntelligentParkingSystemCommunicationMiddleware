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
        protected FileConfig FieldConfig { get; private set; }

        protected Result(String jsonString)
        {
            this.FieldConfig = FileConfig.FindConfig("JsonXPath.cfg");
            var o = JObject.Parse(jsonString);
            this.ResultCode = (ResultCode)o[FieldConfig.GetString("ResultCode")].ToObject<UInt16>();
            this.ErrorMessage = o[FieldConfig.GetString("ErrorMessage")]!=null?o[FieldConfig.GetString("ErrorMessage")].ToString():String.Empty;
        }

    }
}
