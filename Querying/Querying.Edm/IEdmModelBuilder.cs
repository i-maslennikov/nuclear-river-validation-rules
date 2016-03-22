using System;
using System.Collections.Generic;

using Microsoft.OData.Edm;

namespace NuClear.Querying.Edm
{
    public interface IEdmModelBuilder
    {
        IReadOnlyDictionary<Uri, IEdmModel> Build();
    }
}