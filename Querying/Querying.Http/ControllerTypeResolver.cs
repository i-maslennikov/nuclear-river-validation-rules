using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dispatcher;

using NuClear.Querying.Http.Emit;

namespace NuClear.Querying.Http
{
    public sealed class ControllerTypeResolver : DefaultHttpControllerTypeResolver
    {
        private readonly IDynamicAssembliesResolver _dynamicAssembliesResolver;

        public ControllerTypeResolver(IDynamicAssembliesResolver dynamicAssembliesResolver)
        {
            _dynamicAssembliesResolver = dynamicAssembliesResolver;
        }

        public override ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
        {
            var controllerTypes = base.GetControllerTypes(assembliesResolver);

            var dynamicControllerTypes = _dynamicAssembliesResolver
                .GetDynamicAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => IsControllerTypePredicate(x));

            foreach (var dynamicControllerType in dynamicControllerTypes)
            {
                controllerTypes.Add(dynamicControllerType);
            }

            return controllerTypes;
        }
    }
}