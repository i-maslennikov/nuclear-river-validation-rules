namespace NuClear.CustomerIntelligence.Replication
{
    public static class EntityTypeIds
    {
        // Значения, взятые из ERM
        public const int Account = 142;
        public const int BranchOfficeOrganizationUnit = 139;
        public const int Category = 160;
        public const int CategoryFirmAddress = 166;
        public const int CategoryOrganizationUnit = 161;
        public const int Client = 200;
        public const int Contact = 206;
        public const int Firm = 146;
        public const int FirmAddress = 164;
        public const int FirmContact = 165;
        public const int LegalPerson = 147;
        public const int Order = 151;
        public const int CategoryGroup = 162;
        public const int Project = 158;
        public const int Territory = 191;
        public const int SalesModelCategoryRestriction = 272;
        public const int Lead = 582;

        public const int Activity = 500;
        public const int Appointment = 501;
        public const int Phonecall = 502;
        public const int Task = 503;
        public const int Letter = 504;

        // TODO {all, 27.08.2015}: Сущности не имеет отношения к домену поиска, удалить после рефакторинга на стороне ERM
        public const int Building = 241;
        public const int Deal = 199;
        public const int OrderPosition = 150;
        public const int Bill = 188;
        public const int LegalPersonProfile = 219;
        public const int Lock = 159;

        // Значения, первоисточником которых является CI
        //public const int FirmCategoryStatistics = 10010;
        public const int ProjectStatistics = 10012;
        public const int ProjectCategoryStatistics = 10011;
    }
}