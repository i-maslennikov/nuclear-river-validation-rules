using System;

using NuClear.Storage.Core;

namespace NuClear.ValidationRules.Storage
{
    public class DefaultEntityContainerNameResolver : IEntityContainerNameResolver
    {
        private const string Erm = "Erm";
        private const string Facts = "Facts";
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

            if (objType.Name == "PerformedOperationFinalProcessing")
            {
                return Transport;
            }

            throw new ArgumentException($"Unsupported type {objType.Name}: can not determine scope", nameof(objType));
        }
    }
}