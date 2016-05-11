using System;

using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Storage.Core;

namespace NuClear.CustomerIntelligence.Replication.Host.Settings
{
    public class DefaultEntityContainerNameResolver : IEntityContainerNameResolver
    {
        private const string Erm = "Erm";
        private const string Bit = "Bit";
        private const string Facts = "Facts";
        private const string CustomerIntelligence = "CustomerIntelligence";
        private const string Transport = "Transport";

        public string Resolve(Type objType)
        {
            if (objType.Namespace.Contains(Erm))
            {
                return Erm;
            }

            if (objType.Namespace.Contains(Facts))
            {
                return Facts;
            }

            if (objType.Namespace.EndsWith(Bit))
            {
                return Facts;
            }

            if (objType == typeof(PerformedOperationFinalProcessing))
            {
                return Transport;
            }

            return CustomerIntelligence;
        }
    }
}