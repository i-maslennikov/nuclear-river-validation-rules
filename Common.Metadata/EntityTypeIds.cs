namespace NuClear.River.Common.Metadata
{
    public static class EntityTypeIds
    {
        // Значения, взятые из ERM
        public const int Account = 142;
        public const int AssociatedPosition = 177;
        public const int AssociatedPositionsGroup = 176;
        public const int CategoryFirmAddress = 166;
        public const int CategoryOrganizationUnit = 161;
        public const int Client = 200;
        public const int Contact = 206;
        public const int DeniedPosition = 180;
        public const int Firm = 146;
        public const int FirmAddress = 164;
        public const int FirmContact = 165;
        public const int LegalPerson = 147;
        public const int Order = 151;
        public const int OrderPositionAdvertisement = 216;
        public const int CategoryGroup = 162;
        public const int OrganizationUnit = 157;
        public const int Position = 153;
        public const int Project = 158;
        public const int Price = 155;
        public const int PricePosition = 154;
        public const int Territory = 191;
        public const int SalesModelCategoryRestriction = 272;

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
        public const int BranchOfficeOrganizationUnit = 10001;
        public const int Category = 10002;
        public const int FirmBalance = 10003;
        public const int FirmCategoryGroup = 10005;
        public const int ProjectCategory = 10006;
        public const int FirmCategory1 = 10007;
        public const int FirmCategory2 = 10008;
        public const int FirmCategory3 = 10009;

        // Значения, первоисточником которых является VR
        public const int Period = 10101;
    }
}
