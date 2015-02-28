using System;
using IPSCM.Configuration;
using Newtonsoft.Json.Linq;

namespace IPSCM.Protocol.Entities.Results
{
    public class LoginResult : Result
    {
        public String Token { get; private set; }

        public LoginResult(String jsonString)
            : base(jsonString)
        {
            var o = JObject.Parse(jsonString);
            var val = o[this.FieldConfig.GetString("Info")][this.FieldConfig.GetString("Token")];
            this.Token = val != null ? val.ToString() : String.Empty;
        }
    }
}
