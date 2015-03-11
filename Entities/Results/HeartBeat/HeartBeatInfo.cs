using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPSCM.Entities.Results.HeartBeat
{
    public class HeartBeatInfo
    {
        public List<Surplus> Surpluses { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<User> Users { get; set; }
    }
}
