﻿using NuClear.Replication.Core;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.Replication.Tests.Actors
{
    internal abstract class ActorFixtureBase : DataFixtureBase
    {
        protected static class DataObject
        {
            public static IEvent Created<T>(long entityId)
            {
                return new DataObjectCreatedEvent(typeof(T), entityId);
            }

            public static IEvent Updated<T>(long entityId)
            {
                return new DataObjectUpdatedEvent(typeof(T), entityId);
            }

            public static IEvent Deleted<T>(long entityId)
            {
                return new DataObjectDeletedEvent(typeof(T), entityId);
            }

            public static IEvent RelatedDataObjectOutdated<T>(long entityId)
            {
                return new RelatedDataObjectOutdatedEvent<long>(typeof(T), entityId);
            }
        }
    }
}