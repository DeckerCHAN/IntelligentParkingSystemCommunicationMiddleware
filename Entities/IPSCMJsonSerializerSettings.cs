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
