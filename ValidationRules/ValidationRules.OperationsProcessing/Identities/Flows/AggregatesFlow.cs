using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Flows
{
    public sealed class AggregatesFlow : MessageFlowBase<AggregatesFlow>
    {
        public override Guid Id { get; } = new Guid("CB1434CA-D575-4470-8616-4F08D074C8DA");

        public override string Description { get; } = "Маркер для потока выполненных операций в системе обеспечивающих репликацию изменений в домен Validation Rules";
    }
}