using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public class ProcessPrivateMemorySizeIdentity : TelemetryIdentityBase<ProcessPrivateMemorySizeIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "The amount of memory, in bytes, allocated for the associated process that cannot be shared with other processes"; }
        }
    }
}