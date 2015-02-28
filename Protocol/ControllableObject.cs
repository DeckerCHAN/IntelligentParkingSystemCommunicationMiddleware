using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCM.Logging;

namespace IPSCM.Protocol
{
    public delegate void StartEventHandler(object sender);
    public delegate void StopEventHandler(object sender);
    public abstract class ControllableObject
    {
        protected ControllableObject()
        {
            this.RunningStatus = GateStatus.Initialized;
        }
        public event StartEventHandler OnStart;
        public event StopEventHandler OnStop;
        public GateStatus RunningStatus { get; protected set; }

        public virtual void Start()
        {
            if (this.RunningStatus != GateStatus.Initialized)
            {
                Log.Error("Trying to start a controllable object but did not initialized yet or has been stopped. I think this is a bug or logic mistake in code.");
                return;
            }
            this.RunningStatus = GateStatus.Started;
            var triger = this.OnStart;
            if (triger != null) triger(this);
        }

        public virtual void Stop()
        {
            if (this.RunningStatus != GateStatus.Started)
            {
                Log.Error("Trying to stop a controllable object but did not started yet. I think this is a bug or logic mistake in code.");
                return;
            }
            this.RunningStatus = GateStatus.Endded;
            var triger = this.OnStop;
            if (triger != null) triger(this);
        }
    }
}
