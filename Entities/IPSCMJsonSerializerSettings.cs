using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace IPSCM.Entities
{
    public class IPSCMJsonSerializerSettings : JsonSerializerSettings 
    {
        public IPSCMJsonSerializerSettings()
        {
            this.ContractResolver=new IPSCMContractResolver();
        }
    }
}
