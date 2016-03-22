using System;

using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.Querying.Edm.EF
{
    public interface IClrTypeProvider
    {
        Type Get(IMetadataElementIdentity elementIdentity);
    }
}