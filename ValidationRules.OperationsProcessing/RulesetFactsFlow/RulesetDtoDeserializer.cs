using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NuClear.River.Hosting.Common.Settings;
using NuClear.ValidationRules.Replication.Dto;

namespace NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow
{
    public sealed class RulesetDtoDeserializer : IDeserializer<Confluent.Kafka.Message, RulesetDto>
    {
        private readonly string _targetBusinessModel;

        public RulesetDtoDeserializer(IEnvironmentSettings environmentSettings)
        {
            _targetBusinessModel = ExtractBusinessModelSuffix(environmentSettings.EnvironmentName);
        }

        public IReadOnlyCollection<RulesetDto> Deserialize(Confluent.Kafka.Message kafkaMessage)
        {
            // filter tombstone messages
            var kafkaMessagePayload = kafkaMessage.Value;
            if (kafkaMessagePayload == null)
            {
                return Array.Empty<RulesetDto>();
            }

            var rawXmlRulesetMessage = Encoding.UTF8.GetString(kafkaMessagePayload);
            var xmlRulesetMessage = XElement.Parse(rawXmlRulesetMessage);

            var sourceBusinessModel = ExtractBusinessModelSuffix(xmlRulesetMessage.Attribute("SourceCode")?.Value);
            if (String.Compare(sourceBusinessModel, _targetBusinessModel, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                // сообщение предназначено для другой businessmodel
                return Array.Empty<RulesetDto>();
            }

            return new[] { ConvertToRulesetDto(xmlRulesetMessage) };
        }

        private string ExtractBusinessModelSuffix(string rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException(nameof(rawValue));
            }

            var lastIndex = rawValue.LastIndexOf(".", StringComparison.Ordinal);
            return rawValue.Substring(lastIndex, rawValue.Length - lastIndex);
        }

        private static RulesetDto ConvertToRulesetDto(XElement rulesetXml)
        {
            var rulesElements = rulesetXml.Element("Rules");
            return new RulesetDto
                {
                    Id = (long)rulesetXml.Attribute("Code"),
                    BeginDate = (DateTime)rulesetXml.Attribute("BeginDate"),
                    EndDate = (DateTime?)rulesetXml.Attribute("EndDate"),
                    AssociatedRules = rulesElements.Element("Associated")
                                                   .Elements("Rule")
                                                   .Select(Convert2AssociatedRule),
                    DeniedRules = rulesElements.Element("Denied")
                                               .Elements("Rule")
                                               .Select(Convert2DeniedRule),
                    QuantitativeRules = rulesElements.Element("Quantitative")
                                                     .Elements("Rule")
                                                     .Select(Convert2QuantitativeRule),
                    Projects = rulesetXml.Element("Branches")
                                         .Elements("Branch")
                                         .Select(b => (long)b.Attribute("Code"))
                };
        }

        private static RulesetDto.AssociatedRule Convert2AssociatedRule(XElement ruleElement)
        {
            return new RulesetDto.AssociatedRule
                {
                    NomeclatureId = (long)ruleElement.Attribute("PrincipalNomenclatureCode"),
                    AssociatedNomenclatureId = (long)ruleElement.Attribute("AssociatedNomenclatureCode"),
                    ConsideringBindingObject = (bool)ruleElement.Attribute("IsConsiderBindingObject")
                };
        }

        private static RulesetDto.DeniedRule Convert2DeniedRule(XElement ruleElement)
        {
            var nomenclaturesElements = ruleElement.Element("Nomenclatures")
                                                   .Elements("Nomenclature")
                                                   .Select(n => (long)n.Attribute("Code"))
                                                   .ToList();

            if (nomenclaturesElements.Count != 2)
            {
                throw new InvalidOperationException("Denied rule element have to contain exactly 2 nomenclature sub elements");
            }

            return new RulesetDto.DeniedRule
                {
                    NomeclatureId = nomenclaturesElements[0],
                    DeniedNomenclatureId = nomenclaturesElements[1],
                    BindingObjectStrategy = ConvertBindingObjectStrategy(ruleElement.Attribute("BindingTypeStrategy")?.Value)
                };
        }

        private static RulesetDto.QuantitativeRule Convert2QuantitativeRule(XElement ruleElement)
        {
            return new RulesetDto.QuantitativeRule
                {
                    NomenclatureCategoryCode = (long)ruleElement.Attribute("NomenclatureCategoryCode"),
                    Min = (int)ruleElement.Attribute("Min"),
                    Max = (int)ruleElement.Attribute("Max"),
                };
        }

        private static int ConvertBindingObjectStrategy(string rawValue)
        {
            switch (rawValue)
            {
                case "Match":
                    return 1;
                case "NoDependency":
                    return 2;
                case "Different":
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rawValue), rawValue);
            }
        }
    }
}