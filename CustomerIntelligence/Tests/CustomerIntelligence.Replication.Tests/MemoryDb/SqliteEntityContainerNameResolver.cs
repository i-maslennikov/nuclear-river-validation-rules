using System;

using NuClear.Storage.Core;

namespace NuClear.CustomerIntelligence.Replication.Tests.MemoryDb
{
    public class SqliteEntityContainerNameResolver : IEntityContainerNameResolver
    {
        private const string Erm = "Erm";
        private const string Facts = "Facts";
        private const string Bit = "Bit";
        private const string CustomerIntelligence = "CustomerIntelligence";

        public string Resolve(Type objType)
        {
            if (objType.Namespace.EndsWith(Erm))
            {
                return Erm;
            }

            if (objType.Namespace.EndsWith(Facts))
            {
                return Facts;
            }

            if (objType.Namespace.EndsWith(Bit))
            {
                return Facts;
            }

            return CustomerIntelligence;
        }
    }
}