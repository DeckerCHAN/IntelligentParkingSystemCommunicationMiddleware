using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPSCM.Protocol
{
    public delegate void StartEventHandler(object sender);
    public delegate void StopEventHandler(object sender);
    public abstract class ControllableObject
    {
        public ControllableObject()
        {
            
        }
        public event StartEventHandler OnStart;
        public event StopEventHandler OnStop;
        public void Start()
        {
            var triger = this.OnStart;
            if (triger != null) triger(this);
        }

        public void Stop()
        {
            var triger = this.OnStop;
            if (triger != null) triger(this);
        }
    }
}
