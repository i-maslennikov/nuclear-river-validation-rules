using System;
using System.Globalization;
using System.Linq;

using ValidationRules.Replication.Comparison.Tests.ErmService;

namespace ValidationRules.Replication.Comparison.Tests.RiverService
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

        public ErmValidationResult ValidateSingleForCancel(long orderId)
        {
            var response = _riverClient.SingleForCancel(new RiverSingleCheckRequest { OrderId = orderId });
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

        private static ErmOrderValidationMessage[] Format(RiverValidationResult[] results, Func<EntityReference, string> descriptionFormatter)
            => results.Select(x =>
                                  new ErmOrderValidationMessage
                                  {
                                      TargetEntityId = AdaptTargetEntityId(x),
                                      RuleCode = x.Rule,
                                      MessageText = AdaptMessage(x, descriptionFormatter)
                                  }).ToArray();

        // todo: по идее, эта хрень уже не нужна
        private static long AdaptTargetEntityId(RiverValidationResult result)
        {
            switch (result.Rule)
            {
                case 39:
                case 48:
                case 49:
                    return result.References.Single(r => r.Type == "Firm").Id;
                default:
                    return result.MainReference.Id;
            }
        }

        private static string AdaptMessage(RiverValidationResult result, Func<EntityReference, string> descriptionFormatter)
        {
            switch (result.Rule)
            {
                case 20:
                    return string.Format(CultureInfo.InvariantCulture, result.Template, result.References.Select(x => x.Name).ToArray());
                default:
                    return string.Format(CultureInfo.InvariantCulture, result.Template, result.References.Select(descriptionFormatter).ToArray());
            }
        }
    }
}
