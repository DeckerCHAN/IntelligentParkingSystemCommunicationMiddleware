using System;

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
            var triger = this.OnStart;
            if (triger != null) triger(this);
            if (this.RunningStatus != GateStatus.Initialized)
            {
                throw new InvalidOperationException(String.Format("Trying to start a {0} control object", this.RunningStatus));
            }
            this.RunningStatus = GateStatus.Started;
        }

        public virtual void Stop()
        {
            var triger = this.OnStop;
            if (triger != null) triger(this);
            if (this.RunningStatus != GateStatus.Started)
            {
                throw new InvalidOperationException(String.Format("Trying to stop a {0} control object", this.RunningStatus));
            }
            this.RunningStatus = GateStatus.Endded;


        }
    }
}
