using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows
{
    public sealed class StatisticsEventsFlow : MessageFlowBase<StatisticsEventsFlow>
    {
        public override Guid Id => new Guid("EED0A445-4A53-4D49-89F5-01DD440C85C8");

        public override string Description => "Поток изменений, вызывающих пересчёт статистики по рубрике проекта";
    }
}