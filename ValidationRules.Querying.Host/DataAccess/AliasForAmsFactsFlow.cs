using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    /// <summary>
    /// Локальный alias для AmsFactsFlow который не доступен, т.к. нет желания иметь ссылку на OperationsProcessing из-за дескриптора потока
    /// А другого более правильного расположения в не OperationsProcessing для AmsFactsFlow пока не найдено, наиболее подходящий кандидат
    /// ValidationRules.Storage, но тогда его нужно преобразовать в ValidationRules.Model
    /// </summary>
    internal sealed class AliasForAmsFactsFlow : MessageFlowBase<AliasForAmsFactsFlow>
    {
        public override Guid Id => new Guid("A2878E80-992A-4602-8FD6-B10AE85BBFFE");

        public override string Description => nameof(AliasForAmsFactsFlow);
    }
}