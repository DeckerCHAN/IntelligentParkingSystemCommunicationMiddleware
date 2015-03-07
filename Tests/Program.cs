using System;
using IPSCM.Entities;
using IPSCM.Entities.Results;

namespace IPSCM.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            String json = "{\"result_code\": \"200\",\"info\": {\"token\": \"1234567890abcdefgh\"	}}";
            String json2 = "{\"result_code\": \"200\"}";
            var lgr = IPSCMJsonConvert.Parse<LoginResult>(json);
            Console.WriteLine(lgr.ResultCode);
        }
    }
}
