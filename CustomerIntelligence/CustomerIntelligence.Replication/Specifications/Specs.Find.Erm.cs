using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static class Erm
            {
                private const int ActivityStatusCompleted = 2;
                private const int RegardingObjectReference = 1;
                private const int LeadStatusOpen = 1;

                public static FindSpecification<Account> Accounts()
                {
                    return new FindSpecification<Account>(x => !x.IsArchived);
                }

                public static FindSpecification<Category> Categories()
                {
                    return new FindSpecification<Category>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<CategoryGroup> CategoryGroups()
                {
                    return new FindSpecification<CategoryGroup>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits()
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<CategoryFirmAddress> CategoryFirmAddresses()
                {
                    return new FindSpecification<CategoryFirmAddress>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<CategoryOrganizationUnit> CategoryOrganizationUnits()
                {
                    return new FindSpecification<CategoryOrganizationUnit>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Firm> Firms()
                {
                    return new FindSpecification<Firm>(x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                }

                public static FindSpecification<Lead> Leads()
                {
                    return new FindSpecification<Lead>(x => x.Status == LeadStatusOpen);
                }

                public static FindSpecification<FirmAddress> FirmAddresses()
                {
                    return new FindSpecification<FirmAddress>(x => x.IsActive && !x.IsDeleted && !x.ClosedForAscertainment);
                }

                public static FindSpecification<FirmContact> FirmContacts()
                {
                    return new FindSpecification<FirmContact>(x => true);
                }

                public static FindSpecification<LegalPerson> LegalPersons()
                {
                    return new FindSpecification<LegalPerson>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Client> Clients()
                {
                    return new FindSpecification<Client>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Contact> Contacts()
                {
                    return new FindSpecification<Contact>(x => x.IsActive && !x.IsDeleted && !x.IsFired);
                }

                public static FindSpecification<Order> Orders()
                {
                    return new FindSpecification<Order>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Project> Projects()
                {
                    return new FindSpecification<Project>(x => x.IsActive);
                }

                public static FindSpecification<Territory> Territories()
                {
                    return new FindSpecification<Territory>(x => x.IsActive);
                }

                public static FindSpecification<SalesModelCategoryRestriction> SalesModelCategoryRestrictions()
                {
                    return new FindSpecification<SalesModelCategoryRestriction>(x => true);
                }

                public static FindSpecification<Appointment> Appointments()
                {
                    return new FindSpecification<Appointment>(x => x.IsActive && !x.IsDeleted && x.Status == ActivityStatusCompleted);
                }

                public static FindSpecification<AppointmentReference> ClientAppointments()
                {
                    return new FindSpecification<AppointmentReference>(x => x.Reference == RegardingObjectReference && x.ReferencedType == EntityTypeIds.Client);
                }

                public static FindSpecification<AppointmentReference> FirmAppointments()
                {
                    return new FindSpecification<AppointmentReference>(x => x.Reference == RegardingObjectReference && x.ReferencedType == EntityTypeIds.Firm);
                }

                public static FindSpecification<Phonecall> Phonecalls()
                {
                    return new FindSpecification<Phonecall>(x => x.IsActive && !x.IsDeleted && x.Status == ActivityStatusCompleted);
                }

                public static FindSpecification<PhonecallReference> ClientPhonecalls()
                {
                    return new FindSpecification<PhonecallReference>(x => x.Reference == RegardingObjectReference && x.ReferencedType == EntityTypeIds.Client);
                }

                public static FindSpecification<PhonecallReference> FirmPhonecalls()
                {
                    return new FindSpecification<PhonecallReference>(x => x.Reference == RegardingObjectReference && x.ReferencedType == EntityTypeIds.Firm);
                }

                public static FindSpecification<Task> Tasks()
                {
                    return new FindSpecification<Task>(x => x.IsActive && !x.IsDeleted && x.Status == ActivityStatusCompleted);
                }

                public static FindSpecification<TaskReference> ClientTasks()
                {
                    return new FindSpecification<TaskReference>(x => x.Reference == RegardingObjectReference && x.ReferencedType == EntityTypeIds.Client);
                }

                public static FindSpecification<TaskReference> FirmTasks()
                {
                    return new FindSpecification<TaskReference>(x => x.Reference == RegardingObjectReference && x.ReferencedType == EntityTypeIds.Firm);
                }

                public static FindSpecification<Letter> Letters()
                {
                    return new FindSpecification<Letter>(x => x.IsActive && !x.IsDeleted && x.Status == ActivityStatusCompleted);
                }

                public static FindSpecification<LetterReference> ClientLetters()
                {
                    return new FindSpecification<LetterReference>(x => x.Reference == RegardingObjectReference && x.ReferencedType == EntityTypeIds.Client);
                }

                public static FindSpecification<LetterReference> FirmLetters()
                {
                    return new FindSpecification<LetterReference>(x => x.Reference == RegardingObjectReference && x.ReferencedType == EntityTypeIds.Firm);
                }
            }
        }
    }
}