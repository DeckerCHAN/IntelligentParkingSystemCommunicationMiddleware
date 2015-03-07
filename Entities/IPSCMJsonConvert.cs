using System;
using Newtonsoft.Json;

namespace IPSCM.Entities
{
    public static class IPSCMJsonConvert
    {
        public static String ConvertToJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj, new IPSCMJsonSerializerSettings());
        }

        public static T Parse<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json, new IPSCMJsonSerializerSettings());
        }
    }
}
