using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ValidationRules.Replication.SingleCheck.Tests.ErmService;

namespace ValidationRules.Replication.SingleCheck.Tests.RiverService
{
    public sealed class RiverToErmResultAdapter
    {
        private readonly IRiverRestService _riverClient;

        public RiverToErmResultAdapter(string endpointConfigurationName)
        {
            _riverClient = new RiverRestService(endpointConfigurationName);
        }

        public ErmValidationResult ValidateSingle(long orderId)
        {
            var response = _riverClient.Single(new RiverSingleCheckRequest { OrderId = orderId });
            var messages = Format(response, FormatDescriptionSingle);
            return new ErmValidationResult { OrderCount = 1, Messages = messages };
        }

        public ErmValidationResult ValidateMassManual(long[] orderIds, long projectId, DateTime releaseDate)
        {
            var response = _riverClient.Manual(new RiverMassCheckRequest {OrderIds = orderIds, ProjectId = projectId, ReleaseDate = releaseDate});
            var messages = Format(response, FormatDescriptionMass);
            return new ErmValidationResult { OrderCount = orderIds.Length, Messages = messages };
        }

        public ErmValidationResult ValidateMassPrerelease(long[] orderIds, long projectId, DateTime releaseDate)
        {
            var response = _riverClient.Prerelease(new RiverMassCheckRequest { OrderIds = orderIds, ProjectId = projectId, ReleaseDate = releaseDate });
            var messages = Format(response, FormatDescriptionMass);
            return new ErmValidationResult { OrderCount = orderIds.Length, Messages = messages };
        }

        public ErmValidationResult ValidateMassRelease(long[] orderIds, long projectId, DateTime releaseDate)
        {
            var response = _riverClient.Release(new RiverMassCheckRequest { OrderIds = orderIds, ProjectId = projectId, ReleaseDate = releaseDate });
            var messages = Format(response, FormatDescriptionMass);
            return new ErmValidationResult { OrderCount = orderIds.Length, Messages = messages };
        }

        private static string FormatDescriptionSingle(EntityReference entityReference)
            => string.Format(CultureInfo.InvariantCulture, "<{0}:{1}:{2}>", entityReference.Type, entityReference.Name, entityReference.Id);

        private static string FormatDescriptionMass(EntityReference entityReference)
            => entityReference.Name;

        private static readonly HashSet<int> FirmRules = new HashSet<int> { 39, 48, 49, };
        private static ErmOrderValidationMessage[] Format(RiverValidationResult[] results, Func<EntityReference, string> descriptionFormatter)
            => results.Select(x =>
                                  new ErmOrderValidationMessage
                                  {
                                      TargetEntityId = FirmRules.Contains(x.Rule) ? x.References.Single(r => r.Type == "Firm").Id : x.MainReference.Id,
                                      RuleCode = x.Rule,
                                      MessageText = FormatMessage(x, descriptionFormatter)
                                  }).ToArray();

        private static string FormatMessage(RiverValidationResult result, Func<EntityReference, string> descriptionFormatter)
        {
            return string.Format(CultureInfo.InvariantCulture, result.Template, result.References.Select(descriptionFormatter).ToArray());
        }
    }
}
