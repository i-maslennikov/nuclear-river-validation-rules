using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAppointment : EntityTypeBase<EntityTypeAppointment>
    {
        public override int Id
        {
            get { return EntityTypeIds.Appointment; }
        }

        public override string Description
        {
            get { return "Appointment"; }
        }
    }
}