using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.AggregatesFlow
{
    public sealed class AggregatesFlow : MessageFlowBase<AggregatesFlow>
    {
        public override Guid Id => new Guid("CB1434CA-D575-4470-8616-4F08D074C8DA");

        public override string Description => "Маркер для потока выполненных операций в системе обеспечивающих репликацию изменений в домен Validation Rules";
    }
}