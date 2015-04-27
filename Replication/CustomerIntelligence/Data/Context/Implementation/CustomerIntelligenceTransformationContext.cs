﻿using System;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation
{
    public sealed class CustomerIntelligenceTransformationContext : ICustomerIntelligenceContext
    {
        private readonly IFactsContext _context;

        public CustomerIntelligenceTransformationContext(IFactsContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
        }

        public IQueryable<Client> Clients
        {
            get
            {
                // TODO {all, 02.04.2015}: CategoryGroupId processing
                return from client in _context.Clients
                       select new Client
                              {
                                  Id = client.Id,
                                  Name = client.Name,
                                  //CategoryGroupId = null
                              };
            }
        }

        public IQueryable<Contact> Contacts
        {
            get
            {
                return from contact in _context.Contacts
                       select new Contact
                              {
                                  Id = contact.Id,
                                  Role = contact.Role,
                                  IsFired = contact.IsFired,
                                  ClientId = contact.ClientId
                              };
            }
        }

        public IQueryable<Firm> Firms
        {
            get
            {
                // FIXME {all, 03.04.2015}: the obtained SQL is too complex and slow

                var clientsHavingPhone = from contact in _context.Contacts
                                         where contact.HasPhone
                                         select contact.ClientId;
                var clientsHavingWebsite = from contact in _context.Contacts
                                           where contact.HasWebsite
                                           select contact.ClientId;

                var firmsHavingPhone = from firmContact in _context.FirmContacts.Where(x => x.HasPhone)
                                       join firmAddress in _context.FirmAddresses on firmContact.FirmAddressId equals firmAddress.Id
                                       select firmAddress.FirmId;
                var firmsHavingWebsite = from firmContact in _context.FirmContacts.Where(x => x.HasWebsite)
                                         join firmAddress in _context.FirmAddresses on firmContact.FirmAddressId equals firmAddress.Id
                                         select firmAddress.FirmId;

                // TODO {all, 02.04.2015}: CategoryGroupId processing
                return from firm in _context.Firms
                       join client in _context.Clients on firm.ClientId equals client.Id into firmClients
                       from firmClient in firmClients.DefaultIfEmpty()
                       select new Firm
                              {
                                  Id = firm.Id,
                                  Name = firm.Name,
                                  CreatedOn = firm.CreatedOn,
                                  LastDisqualifiedOn = (firmClient != null ? firmClient.LastDisqualifiedOn : firm.LastDisqualifiedOn),
                                  LastDistributedOn = _context.Orders.Where(o => o.FirmId == firm.Id).Select(d => d.EndDistributionDateFact).Cast<DateTimeOffset?>().Max(),
                                  HasPhone = firmsHavingPhone.Contains(firm.Id) || (firmClient != null && firmClient.HasPhone) || (firm.ClientId != null && clientsHavingPhone.Contains(firm.ClientId.Value)),
                                  HasWebsite = firmsHavingWebsite.Contains(firm.Id) || (firmClient != null && firmClient.HasWebsite) || (firm.ClientId != null && clientsHavingWebsite.Contains(firm.ClientId.Value)),
                                  AddressCount = _context.FirmAddresses.Count(fa => fa.FirmId == firm.Id),
                                  //CategoryGroupId = null,
                                  ClientId = firm.ClientId,
                                  OrganizationUnitId = firm.OrganizationUnitId,
                                  OwnerId = firm.OwnerId,
                                  TerritoryId = firm.TerritoryId
                              };
            }
        }

        public IQueryable<FirmBalance> FirmBalances
        {
            get
            {
                return from account in _context.Accounts
                       join legalPerson in _context.LegalPersons on account.LegalPersonId equals legalPerson.Id
                       join client in _context.Clients on legalPerson.ClientId equals client.Id
                       join branchOfficeOrganizationUnit in _context.BranchOfficeOrganizationUnits on account.BranchOfficeOrganizationUnitId equals branchOfficeOrganizationUnit.Id
                       join firm in _context.Firms on branchOfficeOrganizationUnit.OrganizationUnitId equals firm.OrganizationUnitId
                       where firm.ClientId == client.Id
                       select new FirmBalance { AccountId = account.Id, FirmId = firm.Id, Balance = account.Balance };
            }
        }

        public IQueryable<FirmCategory> FirmCategories
        {
            get
            {
                var categories1 = _context.Categories.Where(x => x.Level == 1);
                var categories2 = _context.Categories.Where(x => x.Level == 2);
                var categories3 = _context.Categories.Where(x => x.Level == 3);

                var level3 = from firmAddress in _context.FirmAddresses
                             join categoryFirmAddress in _context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             select new FirmCategory { FirmId = firmAddress.FirmId, CategoryId = category3.Id };

                var level2 = from firmAddress in _context.FirmAddresses
                             join categoryFirmAddress in _context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             select new FirmCategory { FirmId = firmAddress.FirmId, CategoryId = category2.Id };

                var level1 = from firmAddress in _context.FirmAddresses
                             join categoryFirmAddress in _context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                             join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                             join category2 in categories2 on category3.ParentId equals category2.Id
                             join category1 in categories1 on category2.ParentId equals category1.Id
                             select new FirmCategory { FirmId = firmAddress.FirmId, CategoryId = category1.Id };

                // perform union using distinct
                return level3.Union(level2).Union(level1);
            }
        }

        public IQueryable<FirmCategoryGroup> FirmCategoryGroups
        {
            get
            {
                return (from firm in _context.Firms
                        join firmAddress in _context.FirmAddresses on firm.Id equals firmAddress.FirmId
                        join categoryFirmAddress in _context.CategoryFirmAddresses on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                        join categoryOrganizationUnit in _context.CategoryOrganizationUnits on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                        where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                        select new FirmCategoryGroup
                               {
                                   FirmId = firmAddress.FirmId,
                                   CategoryGroupId = categoryOrganizationUnit.CategoryGroupId
                               }).Distinct();
            }
        }
    }
}