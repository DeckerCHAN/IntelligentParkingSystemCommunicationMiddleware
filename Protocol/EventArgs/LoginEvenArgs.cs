using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCM.Protocol.Entities;
using IPSCM.Protocol.Entities.Results;

namespace IPSCM.Protocol.EventArgs
{
    public class LoginEvenArgs : System.EventArgs
    {
        public LoginResult Entity { get; private set; }

        public LoginEvenArgs(LoginResult entity)
        {
            this.Entity = entity;
        }
    }
}
