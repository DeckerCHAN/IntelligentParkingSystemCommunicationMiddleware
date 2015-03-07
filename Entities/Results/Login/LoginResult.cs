using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCM.Entities.Results.Login;

namespace IPSCM.Entities.Results
{
    public class LoginResult : Result
    {
        public LoginInfo Info { get; set; }
    }
}
