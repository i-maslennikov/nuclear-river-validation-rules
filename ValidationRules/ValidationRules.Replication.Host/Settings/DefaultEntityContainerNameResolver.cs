using System;

using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Storage.Core;

namespace NuClear.ValidationRules.Replication.Host.Settings
{
    public class DefaultEntityContainerNameResolver : IEntityContainerNameResolver
    {
        private const string Erm = "Erm";
        private const string Facts = "Facts";
        private const string Aggregates = "Aggregates";
        private const string Transport = "Transport";
        private const string Messages = "Messages";

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

            if (objType.Namespace.Contains(Aggregates))
            {
                return Aggregates;
            }

            if (objType.Namespace.Contains(Messages))
            {
                return Messages;
            }

            if (objType == typeof(PerformedOperationFinalProcessing))
            {
                return Transport;
            }

            throw new ArgumentException($"Unsupported type {objType.Name}: can not determine scope", nameof(objType));
        }
    }
}