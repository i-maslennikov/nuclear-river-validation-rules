using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.StateInitialization.Host;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests.Infrastructure
{
    /// <summary>
    /// Данный тип - замена реализации из пакета NuClear.DataTests, для его подключения понадобилось подменить также CreateDatabaseSchemataCommand (т.к. ctor содержит в зависимостях конкретный тип вместо абстракции)
    /// Причина замены - при штатном поведении NuClear.DataTests, таблицы в localdb создаются только для тех типов сущностей, для которых настроены тестовые данные, что вроде бы логично.
    /// Однако, если в тестовом наборе есть хоть один тест для конкретного этапа процессинга (Erm->Facts, Facts->Aggregates, Aggregates->Messages) - в качестве
    /// целевых типов агрегатов будут приняты все типы возвращаемые DataObjectTypesProviderFactory для конкретного контекста процессинга, соответственно, будут вызваны их accesors,
    /// которые будут падать если для типов используемых в запросах accessor не созданы таблицы, что вполне возможно, т.к. для каких-то стеков агрегатов, вполне могут отсутствовать
    /// тесты на один из этапов процессинга.
    /// Альтернативный варианты:
    ///     - всегда для всего используемого в запросах реализаций, производных от IStorage*Accessor настраивать тестовые данные (чтобы создавались таблицы)
    ///     - добиваться фильтрации запускаемых accessor, исключив запуск тех, для которых точно нет настроенных тестовых данных
    ///         - для каждого конкретного теста (тут проблемма сигнатуры ITestAction - туда не передаются детали о конкретном выполняемом тесте)
    ///         - для всех типов упомянутых хотя в одном из тестов одного из контекстов
    ///         В обоих случаях нужно будет передавать в BulkReplicationActor кастомный IAccessorTypesProvider, который будет в зависимости от запрашиваемого типа, отдавать пустой или нет список AccessorTypes
    /// </summary>
    public sealed class ContextEntityTypesProvider : IContextEntityTypesProvider
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyCollection<Type>> _entitiesByContext;

        public ContextEntityTypesProvider()
        {
            var ermEntityAnchorType = typeof(Storage.Model.Erm.Order);
            var ermTypes = ermEntityAnchorType.Assembly
                                              .ExportedTypes
                                              .Where(t => !t.IsInterface
                                                          && !t.IsAbstract
                                                          && !t.IsGenericTypeDefinition
                                                          && t.Namespace == ermEntityAnchorType.Namespace)
                                              .ToList();

            _entitiesByContext = new Dictionary<string, IReadOnlyCollection<Type>>
                {
                    [ContextName.Erm] = ermTypes,
                    [ContextName.Facts] = DataObjectTypesProviderFactory.FactTypes
                                                                        .Union(DataObjectTypesProviderFactory.AmsFactTypes)
                                                                        .Union(DataObjectTypesProviderFactory.RulesetFactTypes)
                                                                        .ToList(),
                    [ContextName.Aggregates] = DataObjectTypesProviderFactory.AggregateTypes,
                    [ContextName.Messages] = DataObjectTypesProviderFactory.MessagesTypes
                };
        }

        public IReadOnlyCollection<Type> GetTypesFromContext(string context)
        {
            if (_entitiesByContext.TryGetValue(context, out var resolvedTypes))
            {
                return resolvedTypes;
            }

            throw new ArgumentException($"Context {context} is unknown. Available ones are: {string.Join(", ", _entitiesByContext.Keys)}");
        }
    }
}