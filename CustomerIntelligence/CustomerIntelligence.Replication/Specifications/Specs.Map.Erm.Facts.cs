using System.Linq;

using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

using Account = NuClear.CustomerIntelligence.Storage.Model.Facts.Account;
using BranchOfficeOrganizationUnit = NuClear.CustomerIntelligence.Storage.Model.Facts.BranchOfficeOrganizationUnit;
using Category = NuClear.CustomerIntelligence.Storage.Model.Facts.Category;
using CategoryFirmAddress = NuClear.CustomerIntelligence.Storage.Model.Facts.CategoryFirmAddress;
using CategoryGroup = NuClear.CustomerIntelligence.Storage.Model.Facts.CategoryGroup;
using CategoryOrganizationUnit = NuClear.CustomerIntelligence.Storage.Model.Facts.CategoryOrganizationUnit;
using Client = NuClear.CustomerIntelligence.Storage.Model.Facts.Client;
using Contact = NuClear.CustomerIntelligence.Storage.Model.Facts.Contact;
using Firm = NuClear.CustomerIntelligence.Storage.Model.Facts.Firm;
using Lead = NuClear.CustomerIntelligence.Storage.Model.Facts.Lead;
using FirmAddress = NuClear.CustomerIntelligence.Storage.Model.Facts.FirmAddress;
using FirmContact = NuClear.CustomerIntelligence.Storage.Model.Facts.FirmContact;
using LegalPerson = NuClear.CustomerIntelligence.Storage.Model.Facts.LegalPerson;
using Order = NuClear.CustomerIntelligence.Storage.Model.Facts.Order;
using Project = NuClear.CustomerIntelligence.Storage.Model.Facts.Project;
using SalesModelCategoryRestriction = NuClear.CustomerIntelligence.Storage.Model.Facts.SalesModelCategoryRestriction;
using Territory = NuClear.CustomerIntelligence.Storage.Model.Facts.Territory;

namespace NuClear.CustomerIntelligence.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Erm
            {
                public static class ToFacts
                {
                    private static readonly int ReserveUserId = 27;

                    public static readonly MapSpecification<IQuery, IQueryable<Activity>> Activities =
                        new MapSpecification<IQuery, IQueryable<Activity>>(
                            q =>
                            {
                                var appointmentActivities = MapToActivity(q.For(Find.Erm.Appointments()), q.For(Find.Erm.FirmAppointments()), q.For(Find.Erm.ClientAppointments()));
                                var phonecallActivities = MapToActivity(q.For(Find.Erm.Phonecalls()), q.For(Find.Erm.FirmPhonecalls()), q.For(Find.Erm.ClientPhonecalls()));
                                var taskActivities = MapToActivity(q.For(Find.Erm.Tasks()), q.For(Find.Erm.FirmTasks()), q.For(Find.Erm.ClientTasks()));
                                var letterActivities = MapToActivity(q.For(Find.Erm.Letters()), q.For(Find.Erm.FirmLetters()), q.For(Find.Erm.ClientLetters()));

                                return appointmentActivities.Union(phonecallActivities).Union(taskActivities).Union(letterActivities);
                            });

                    public static readonly MapSpecification<IQuery, IQueryable<Account>> Accounts =
                        new MapSpecification<IQuery, IQueryable<Account>>(
                            q => from account in q.For(Find.Erm.Accounts())
                                 select new Account
                                 {
                                     Id = account.Id,
                                     Balance = account.Balance,
                                     BranchOfficeOrganizationUnitId = account.BranchOfficeOrganizationUnitId,
                                     LegalPersonId = account.LegalPersonId,
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<BranchOfficeOrganizationUnit>> BranchOfficeOrganizationUnits =
                        new MapSpecification<IQuery, IQueryable<BranchOfficeOrganizationUnit>>(
                            q => from branchOfficeOrganizationUnit in q.For(Find.Erm.BranchOfficeOrganizationUnits())
                                 select new BranchOfficeOrganizationUnit
                                 {
                                     Id = branchOfficeOrganizationUnit.Id,
                                     OrganizationUnitId = branchOfficeOrganizationUnit.OrganizationUnitId
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Category>> Categories =
                        new MapSpecification<IQuery, IQueryable<Category>>(
                            q => from category in q.For(Find.Erm.Categories())
                                 select new Category
                                 {
                                     Id = category.Id,
                                     Name = category.Name,
                                     Level = category.Level,
                                     ParentId = category.ParentId
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<CategoryGroup>> CategoryGroups =
                        new MapSpecification<IQuery, IQueryable<CategoryGroup>>(
                            q => from categoryGroup in q.For(Find.Erm.CategoryGroups())
                                 select new CategoryGroup
                                 {
                                     Id = categoryGroup.Id,
                                     Name = categoryGroup.Name,
                                     Rate = (float)categoryGroup.Rate
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<CategoryFirmAddress>> CategoryFirmAddresses =
                        new MapSpecification<IQuery, IQueryable<CategoryFirmAddress>>(
                            q => from categoryFirmAddress in q.For(Find.Erm.CategoryFirmAddresses())
                                 select new CategoryFirmAddress
                                 {
                                     Id = categoryFirmAddress.Id,
                                     CategoryId = categoryFirmAddress.CategoryId,
                                     FirmAddressId = categoryFirmAddress.FirmAddressId,
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<CategoryOrganizationUnit>> CategoryOrganizationUnits =
                        new MapSpecification<IQuery, IQueryable<CategoryOrganizationUnit>>(
                            q => from categoryOrganizationUnit in q.For(Find.Erm.CategoryOrganizationUnits())
                                 select new CategoryOrganizationUnit
                                 {
                                     Id = categoryOrganizationUnit.Id,
                                     CategoryId = categoryOrganizationUnit.CategoryId,
                                     CategoryGroupId = categoryOrganizationUnit.CategoryGroupId,
                                     OrganizationUnitId = categoryOrganizationUnit.OrganizationUnitId,
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Client>> Clients =
                        new MapSpecification<IQuery, IQueryable<Client>>(
                            q => from client in q.For(Find.Erm.Clients())
                                 select new Client
                                 {
                                     Id = client.Id,
                                     Name = client.Name,
                                     LastDisqualifiedOn = client.LastDisqualifyTime,
                                     HasPhone = (client.MainPhoneNumber ?? client.AdditionalPhoneNumber1 ?? client.AdditionalPhoneNumber2) != null,
                                     HasWebsite = client.Website != null
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Contact>> Contacts =
                        new MapSpecification<IQuery, IQueryable<Contact>>(
                            q => from contact in q.For(Find.Erm.Contacts())
                                 select new Contact
                                 {
                                     Id = contact.Id,
                                     Role = ConvertAccountRole(contact.Role),
                                     HasPhone = (contact.MainPhoneNumber ?? contact.MobilePhoneNumber ?? contact.HomePhoneNumber ?? contact.AdditionalPhoneNumber) != null,
                                     HasWebsite = contact.Website != null,
                                     ClientId = contact.ClientId
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Firm>> Firms =
                        new MapSpecification<IQuery, IQueryable<Firm>>(
                            q => from firm in q.For(Find.Erm.Firms())
                                 select new Firm
                                 {
                                     Id = firm.Id,
                                     Name = firm.Name,
                                     CreatedOn = firm.CreatedOn,
                                     LastDisqualifiedOn = firm.LastDisqualifyTime,
                                     ClientId = firm.ClientId,
                                     OrganizationUnitId = firm.OrganizationUnitId,
                                     OwnerId = firm.OwnerId
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<FirmAddress>> FirmAddresses =
                        new MapSpecification<IQuery, IQueryable<FirmAddress>>(
                            q => from firmAddress in q.For(Find.Erm.FirmAddresses())
                                 select new FirmAddress
                                 {
                                     Id = firmAddress.Id,
                                     FirmId = firmAddress.FirmId,
                                     TerritoryId = firmAddress.TerritoryId
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<FirmContact>> FirmContacts =
                        new MapSpecification<IQuery, IQueryable<FirmContact>>(
                            q => from firmContact in q.For(Find.Erm.FirmContacts())
                                 where firmContact.FirmAddressId != null && (firmContact.ContactType == ContactType.Phone || firmContact.ContactType == ContactType.Website)
                                 select new FirmContact
                                 {
                                     Id = firmContact.Id,
                                     HasPhone = firmContact.ContactType == ContactType.Phone,
                                     HasWebsite = firmContact.ContactType == ContactType.Website,
                                     FirmAddressId = firmContact.FirmAddressId.Value,
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Lead>> Leads =
                        new MapSpecification<IQuery, IQueryable<Lead>>(
                            q => from lead in q.For(Find.Erm.Leads())
                                 where lead.FirmId != null
                                 select new Lead
                                 {
                                     Id = lead.Id,
                                     FirmId = lead.FirmId.Value,
                                     IsInQueue = lead.OwnerId == ReserveUserId,
                                     Type = lead.Type
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<LegalPerson>> LegalPersons =
                        new MapSpecification<IQuery, IQueryable<LegalPerson>>(
                            q => from legalPerson in q.For(Find.Erm.LegalPersons())
                                 where legalPerson.ClientId != null
                                 select new LegalPerson
                                 {
                                     Id = legalPerson.Id,
                                     ClientId = legalPerson.ClientId.Value,
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Order>> Orders =
                        new MapSpecification<IQuery, IQueryable<Order>>(
                            q => from order in q.For(Find.Erm.Orders())
                                 where new[] { OrderState.OnTermination, OrderState.Archive }.Contains(order.WorkflowStepId)
                                 select new Order
                                 {
                                     Id = order.Id,
                                     EndDistributionDateFact = order.EndDistributionDateFact,
                                     FirmId = order.FirmId,
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Project>> Projects =
                        new MapSpecification<IQuery, IQueryable<Project>>(
                            q => from project in q.For(Find.Erm.Projects())
                                 where project.OrganizationUnitId != null
                                 select new Project
                                 {
                                     Id = project.Id,
                                     Name = project.Name,
                                     OrganizationUnitId = project.OrganizationUnitId.Value
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Territory>> Territories =
                        new MapSpecification<IQuery, IQueryable<Territory>>(
                            q => from territory in q.For(Find.Erm.Territories())
                                 select new Territory
                                 {
                                     Id = territory.Id,
                                     Name = territory.Name,
                                     OrganizationUnitId = territory.OrganizationUnitId
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<SalesModelCategoryRestriction>> SalesModelCategoryRestrictions =
                        new MapSpecification<IQuery, IQueryable<SalesModelCategoryRestriction>>(
                            q => from x in q.For(Find.Erm.SalesModelCategoryRestrictions())
                                 select new SalesModelCategoryRestriction
                                 {
                                     Id = x.Id,
                                     CategoryId = x.CategoryId,
                                     ProjectId = x.ProjectId,
                                     SalesModel = x.SalesModel
                                 });

                    private static IQueryable<Activity> MapToActivity<T, TReference>(
                        IQueryable<T> activities,
                        IQueryable<TReference> firmReferences,
                        IQueryable<TReference> clientReferences)
                        where T : ActivityBase
                        where TReference : ActivityReference
                    {
                        // TODO {all, 19.08.2015}: Используется FirstOrDefault вместо DefaultIdEmpty из-за бага в данных Erm
                        // В Erm есть Activity, у которых более одного клиента/фирмы в RegardingObjects. Вероятно, во время миграции.
                        // UI отображает первого из многих, а какой из них реальный - сейчас уже не выяснить.
                        // Если найдётся кто-нибудь, кто удалит лишние данные из erm - можно заменить на left join.
                        return from activity in activities
                               let firmReference = firmReferences.Where(x => x.ActivityId == activity.Id).Select(x => (long?)x.ReferencedObjectId).FirstOrDefault()
                               let clientReference = clientReferences.Where(x => x.ActivityId == activity.Id).Select(x => (long?)x.ReferencedObjectId).FirstOrDefault()
                               select new Activity
                                      {
                                          Id = activity.Id,
                                          ModifiedOn = activity.ModifiedOn,
                                          FirmId = firmReference,
                                          ClientId = clientReference
                                      };
                    }

                    private static int ConvertAccountRole(int value)
                    {
                        switch (value)
                        {
                            case 200000:
                                return 1;
                            case 200001:
                                return 2;
                            case 200002:
                                return 3;
                            default:
                                return 0;
                        }
                    }

                    private static class ContactType
                    {
                        public const int Phone = 1;
                        public const int Website = 4;
                    }

                    private static class OrderState
                    {
                        public const int OnTermination = 4;
                        public const int Archive = 6;
                    }
                }
            }
        }
    }
}