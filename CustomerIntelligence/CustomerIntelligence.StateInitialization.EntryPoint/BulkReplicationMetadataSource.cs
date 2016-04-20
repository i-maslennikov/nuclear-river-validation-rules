using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Storage;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.Bulk.API.Metadata;

namespace NuClear.CustomerIntelligence.StateInitialization.EntryPoint
{
    public sealed class BulkReplicationMetadataSource : MetadataSourceBase<BulkReplicationMetadataKindIdentity>
    {
        private static readonly IReadOnlyDictionary<Uri, IMetadataElement> Elements =
            new BulkReplicationMetadataElement[]
                {
                    BulkReplicationMetadataElement.Config
                                                  .CommandlineKey("-fact")
                                                  .From(ConnectionString.Erm, Schema.Erm)
                                                  .To(ConnectionString.Facts, Schema.Facts)
                                                  .ForDataObject<Domain.Model.Facts.Activity>()
                                                  .ForDataObject<Domain.Model.Facts.Firm>()
                                                  .ForDataObject<Domain.Model.Facts.Account>()
                                                  .ForDataObject<Domain.Model.Facts.BranchOfficeOrganizationUnit>()
                                                  .ForDataObject<Domain.Model.Facts.Category>()
                                                  .ForDataObject<Domain.Model.Facts.CategoryFirmAddress>()
                                                  .ForDataObject<Domain.Model.Facts.CategoryGroup>()
                                                  .ForDataObject<Domain.Model.Facts.CategoryOrganizationUnit>()
                                                  .ForDataObject<Domain.Model.Facts.Client>()
                                                  .ForDataObject<Domain.Model.Facts.Contact>()
                                                  .ForDataObject<Domain.Model.Facts.Firm>()
                                                  .ForDataObject<Domain.Model.Facts.FirmAddress>()
                                                  .ForDataObject<Domain.Model.Facts.FirmContact>()
                                                  .ForDataObject<Domain.Model.Facts.LegalPerson>()
                                                  .ForDataObject<Domain.Model.Facts.Order>()
                                                  .ForDataObject<Domain.Model.Facts.Project>()
                                                  .ForDataObject<Domain.Model.Facts.Territory>()
                                                  .ForDataObject<Domain.Model.Facts.SalesModelCategoryRestriction>(),

                    BulkReplicationMetadataElement.Config
                                                  .CommandlineKey("-ci")
                                                  .From(ConnectionString.Facts, Schema.Facts)
                                                  .To(ConnectionString.CustomerIntelligence, Schema.CustomerIntelligence)
                                                  .ForDataObject<Domain.Model.CI.Firm>()
                                                  .ForDataObject<Domain.Model.CI.FirmActivity>()
                                                  .ForDataObject<Domain.Model.CI.FirmBalance>()
                                                  .ForDataObject<Domain.Model.CI.FirmCategory1>()
                                                  .ForDataObject<Domain.Model.CI.FirmCategory2>()
                                                  .ForDataObject<Domain.Model.CI.FirmTerritory>()
                                                  .ForDataObject<Domain.Model.CI.Client>()
                                                  .ForDataObject<Domain.Model.CI.ClientContact>()
                                                  .ForDataObject<Domain.Model.CI.ProjectCategory>()
                                                  .ForDataObject<Domain.Model.CI.Territory>()
                                                  .ForDataObject<Domain.Model.CI.CategoryGroup>()
                }.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata => Elements;
    }
}