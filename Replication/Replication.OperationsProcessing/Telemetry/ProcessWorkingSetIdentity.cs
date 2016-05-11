using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public class ProcessWorkingSetIdentity : TelemetryIdentityBase<ProcessWorkingSetIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "The amount of physical memory, in bytes, allocated for the associated process"; }
        }
    }
}