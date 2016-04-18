using System;

using NuClear.Storage.Core;

namespace NuClear.CustomerIntelligence.Storage
{
    public class CustomerIntelligenceEntityContainerNameResolver : IEntityContainerNameResolver
    {
        public string Resolve(Type objType)
        {
            return CustomerIntelligenceEntityContainer.Name;
        }
    }
}