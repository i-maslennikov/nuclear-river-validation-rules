if not exists (select * from sys.schemas where name = 'Facts') exec('create schema Facts')
go

if object_id('Facts.EntityName') is not null drop table Facts.EntityName

if object_id('Facts.Account') is not null drop table Facts.Account
if object_id('Facts.Advertisement') is not null drop table Facts.Advertisement
if object_id('Facts.AdvertisementElement') is not null drop table Facts.AdvertisementElement
if object_id('Facts.AdvertisementElementTemplate') is not null drop table Facts.AdvertisementElementTemplate
if object_id('Facts.AdvertisementTemplate') is not null drop table Facts.AdvertisementTemplate
if object_id('Facts.AssociatedPosition') is not null drop table Facts.AssociatedPosition
if object_id('Facts.AssociatedPositionsGroup') is not null drop table Facts.AssociatedPositionsGroup
if object_id('Facts.Bargain') is not null drop table Facts.Bargain
if object_id('Facts.BargainScanFile') is not null drop table Facts.BargainScanFile
if object_id('Facts.Bill') is not null drop table Facts.Bill
if object_id('Facts.BranchOffice') is not null drop table Facts.BranchOffice
if object_id('Facts.BranchOfficeOrganizationUnit') is not null drop table Facts.BranchOfficeOrganizationUnit
if object_id('Facts.Category') is not null drop table Facts.Category
if object_id('Facts.CategoryOrganizationUnit') is not null drop table Facts.CategoryOrganizationUnit
if object_id('Facts.CostPerClickCategoryRestriction') is not null drop table Facts.CostPerClickCategoryRestriction
if object_id('Facts.Deal') is not null drop table Facts.Deal
if object_id('Facts.DeniedPosition') is not null drop table Facts.DeniedPosition
if object_id('Facts.Firm') is not null drop table Facts.Firm
if object_id('Facts.FirmAddress') is not null drop table Facts.FirmAddress
if object_id('Facts.FirmAddressCategory') is not null drop table Facts.FirmAddressCategory
if object_id('Facts.FirmAddressWebsite') is not null drop table Facts.FirmAddressWebsite
if object_id('Facts.LegalPerson') is not null drop table Facts.LegalPerson
if object_id('Facts.LegalPersonProfile') is not null drop table Facts.LegalPersonProfile
if object_id('Facts.Lock') is not null drop table Facts.Lock
if object_id('Facts.NomenclatureCategory') is not null drop table Facts.NomenclatureCategory
if object_id('Facts.[Order]') is not null drop table Facts.[Order]
if object_id('Facts.OrderItem') is not null drop table Facts.OrderItem
if object_id('Facts.OrderPosition') is not null drop table Facts.OrderPosition
if object_id('Facts.OrderPositionAdvertisement') is not null drop table Facts.OrderPositionAdvertisement
if object_id('Facts.OrderPositionCostPerClick') is not null drop table Facts.OrderPositionCostPerClick
if object_id('Facts.OrderScanFile') is not null drop table Facts.OrderScanFile
if object_id('Facts.Position') is not null drop table Facts.Position
if object_id('Facts.PositionChild') is not null drop table Facts.PositionChild
if object_id('Facts.Price') is not null drop table Facts.Price
if object_id('Facts.PricePosition') is not null drop table Facts.PricePosition
if object_id('Facts.Project') is not null drop table Facts.Project
if object_id('Facts.ReleaseInfo') is not null drop table Facts.ReleaseInfo
if object_id('Facts.ReleaseWithdrawal') is not null drop table Facts.ReleaseWithdrawal
if object_id('Facts.RulesetRule') is not null drop table Facts.RulesetRule
if object_id('Facts.SalesModelCategoryRestriction') is not null drop table Facts.SalesModelCategoryRestriction
if object_id('Facts.Theme') is not null drop table Facts.Theme
if object_id('Facts.ThemeCategory') is not null drop table Facts.ThemeCategory
if object_id('Facts.ThemeOrganizationUnit') is not null drop table Facts.ThemeOrganizationUnit
if object_id('Facts.UnlimitedOrder') is not null drop table Facts.UnlimitedOrder

go

create table Facts.EntityName(
    Id bigint not null,
    EntityType int not null,
    [Name] nvarchar(512) not null,
    constraint PK_EntityName primary key (Id, EntityType)
)

create table Facts.Account(
    Id bigint not null,
    BranchOfficeOrganizationUnitId bigint not null,
    LegalPersonId bigint not null,
    Balance decimal(19,4) not null,
    constraint PK_Account primary key (Id)
)

create table Facts.Advertisement (
    Id bigint not null,
    FirmId bigint null,
    AdvertisementTemplateId bigint not null,
    IsSelectedToWhiteList bit not null,
    IsDeleted bit not null,
    constraint PK_Advertisement primary key (Id)
)
go
CREATE INDEX IX_Advertisement_IsDeleted ON [Facts].[Advertisement] ([IsDeleted]) INCLUDE ([Id])

create table Facts.AdvertisementElement (
    Id bigint not null,
    AdvertisementId bigint not null,
    AdvertisementElementTemplateId bigint not null,
    IsEmpty bit not null,
    [Text] nvarchar(max) null,
    [BeginDate] datetime2(2) null,
    [EndDate] datetime2(2) null,
    [Status] int not null,
    constraint PK_AdvertisementElement primary key (Id)
)
go

CREATE INDEX IX_AdvertisementElement_AdvertisementElementTemplateId_IsEmpty ON Facts.[AdvertisementElement] ([AdvertisementElementTemplateId],[IsEmpty]) INCLUDE ([Id],[AdvertisementId])
CREATE INDEX IX_AdvertisementElement_AdvertisementElementTemplateId_Status ON Facts.[AdvertisementElement] (AdvertisementElementTemplateId,[Status]) INCLUDE ([Id],[AdvertisementId])

create table Facts.AdvertisementElementTemplate (
    Id bigint not null,
    IsRequired bit not null,
    NeedsValidation bit not null,
    IsAdvertisementLink bit not null,
    constraint PK_AdvertisementElementTemplate primary key (Id)
)
go

create table Facts.AdvertisementTemplate (
    Id bigint not null,
    DummyAdvertisementId bigint not null,
    IsAdvertisementRequired bit not null,
    IsAllowedToWhiteList bit not null,
    constraint PK_AdvertisementTemplate primary key (Id)
)
go

create table Facts.AssociatedPosition(
    Id bigint not null,
    AssociatedPositionsGroupId bigint not null,
    PositionId bigint not null,
    ObjectBindingType int not null,
    constraint PK_AssociatedPosition primary key (Id)
)
create index IX_AssociatedPosition_AssociatedPositionsGroupId ON Facts.AssociatedPosition (AssociatedPositionsGroupId)
go

create table Facts.AssociatedPositionsGroup(
    Id bigint not null,
    PricePositionId bigint not null,
    constraint PK_AssociatedPositionsGroup primary key (Id)
)
create index IX_AssociatedPositionsGroup_PricePositionId ON Facts.AssociatedPositionsGroup (PricePositionId)
go

create table Facts.Bargain (
    Id bigint not null,
    SignupDate datetime2(2) not null,
    constraint PK_Bargain primary key (Id)
)
go

create table Facts.BargainScanFile (
    Id bigint not null,
    BargainId bigint not null,
    constraint PK_BargainScanFile primary key (Id)
)
go

create table Facts.Bill (
    Id bigint not null,
    OrderId bigint not null,
    PayablePlan decimal(19,4) not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
    constraint PK_Bill primary key (Id)
)
go

create table Facts.BranchOffice (
    Id bigint not null,
    constraint PK_BranchOffice primary key (Id)
)
go

create table Facts.BranchOfficeOrganizationUnit (
    Id bigint not null,
    BranchOfficeId bigint not null,
    constraint PK_BranchOfficeOrganizationUnit primary key (Id)
)
go

create table Facts.Category(
    Id bigint not null,

    L1Id bigint null,
    L2Id bigint null,
    L3Id bigint null,
    IsActiveNotDeleted bit not null,
    constraint PK_Category primary key (Id)
)
go

create table Facts.CategoryOrganizationUnit(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    CategoryId bigint not null,
)
go

create table Facts.CostPerClickCategoryRestriction(
    ProjectId bigint not null,
    CategoryId bigint not null,
    [Begin] datetime2(2) not null,
    MinCostPerClick decimal(19,4) not null,
)
go

create table Facts.Deal (
    Id bigint not null,
    constraint PK_Deal primary key (Id)
)
go

create table Facts.DeniedPosition(
    Id bigint not null,
    PositionId bigint not null,
    PositionDeniedId bigint not null,
    ObjectBindingType int not null,
    PriceId bigint not null,
    constraint PK_DeniedPosition primary key (Id)
)
create index IX_DeniedPosition_PriceId ON Facts.DeniedPosition (PriceId)
go

create table Facts.Firm(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    IsActive bit not null,
    IsDeleted bit not null,
    IsClosedForAscertainment bit not null,
    constraint PK_Firm primary key (Id)
)
go
CREATE NONCLUSTERED INDEX IX_Firm_Id ON [Facts].[Firm] ([Id]) INCLUDE ([IsClosedForAscertainment],[IsActive],[IsDeleted])
GO

create table Facts.FirmAddress(
    Id bigint not null,
    FirmId bigint not null,
    IsLocatedOnTheMap bit not null,
    IsActive bit not null,
    IsDeleted bit not null,
    IsClosedForAscertainment bit not null,
)
go
CREATE NONCLUSTERED INDEX IX_FirmAddress_Id ON [Facts].[FirmAddress] ([Id])
CREATE INDEX [IX_FirmAddress_FirmId_IsActive_IsDeleted_IsClosedForAscertainment] ON [Facts].[FirmAddress] ([FirmId], [IsActive], [IsDeleted], [IsClosedForAscertainment]) INCLUDE (Id)
GO

create table Facts.FirmAddressCategory(
    Id bigint not null,
    FirmAddressId bigint not null,
    CategoryId bigint not null,
    constraint PK_FirmAddressCategory primary key (Id, CategoryId)
)
go
CREATE INDEX IX_FirmAddressCategory_CategoryId ON [Facts].[FirmAddressCategory] ([CategoryId]) INCLUDE ([FirmAddressId])
CREATE INDEX IX_FirmAddressCategory_FirmAddressId_CategoryId ON [Facts].[FirmAddressCategory] ([FirmAddressId],[CategoryId])
GO

create table Facts.FirmAddressWebsite (
    Id bigint not null,
    FirmAddressId bigint not null,
    Website nvarchar(256) not null,
    constraint PK_FirmAddressWebsite primary key (Id)
)
go

create table Facts.LegalPerson(
    Id bigint not null,
    constraint PK_LegalPerson primary key (Id)
)
go

create table Facts.LegalPersonProfile (
    Id bigint not null,
    LegalPersonId bigint not null,
    BargainEndDate datetime2(2) null,
    WarrantyEndDate datetime2(2) null,
    constraint PK_LegalPersonProfile primary key (Id)
)
go
CREATE INDEX IX_LegalPersonProfile_LegalPersonId ON [Facts].[LegalPersonProfile] ([LegalPersonId]) INCLUDE ([Id])
GO

create table Facts.Lock(
    Id bigint not null,
    OrderId bigint not null,
    IsOrderFreeOfCharge bit not null,
    AccountId bigint not null,
    [Start] datetime2(2) not null,
    Amount decimal(19,4) not null,
    constraint PK_Lock primary key (Id)
)
go

create table Facts.NomenclatureCategory(
    Id bigint not null,
    PriceId bigint not null,
    Name nvarchar(256) not null,
    constraint PK_NomenclatureCategory primary key (Id, PriceId)
)
go

create table Facts.[Order](
    Id bigint not null,
    FirmId bigint not null,
    DestOrganizationUnitId bigint not null,
    BeginDistribution datetime2(2) not null,
    EndDistributionPlan datetime2(2) not null,
    EndDistributionFact datetime2(2) not null,
    SignupDate datetime2(2) not null,
    LegalPersonId bigint null,
    LegalPersonProfileId bigint null,
    BranchOfficeOrganizationUnitId bigint null,
    InspectorId bigint null,
    CurrencyId bigint null,
    BargainId bigint null,
    DealId bigint null,
    WorkflowStep int not null,
    IsFreeOfCharge bit not null,
    IsSelfAds bit not null,
    ReleaseCountPlan int not null,

    constraint PK_Order primary key (Id)
)
go
CREATE INDEX IX_Order_DestOrganizationUnitId ON [Facts].[Order] ([DestOrganizationUnitId]) INCLUDE ([Id],[FirmId], [BeginDistribution],[EndDistributionFact],[EndDistributionPlan],[WorkflowStep])
CREATE INDEX IX_Order_LegalPersonId_SignupDate ON [Facts].[Order] ([LegalPersonId],[SignupDate]) INCLUDE ([Id])
CREATE INDEX IX_Order_BargainId ON [Facts].[Order] ([BargainId]) INCLUDE ([Id])
CREATE INDEX IX_Order_BargainId_SignupDate ON [Facts].[Order] ([BargainId],[SignupDate]) INCLUDE ([Id])
CREATE INDEX IX_Order_BeginDistribution on [Facts].[Order]([BeginDistribution])
CREATE INDEX IX_Order_EndDistributionFact on [Facts].[Order]([EndDistributionFact])
CREATE INDEX IX_Order_EndDistributionPlan on [Facts].[Order]([EndDistributionPlan])
GO

create table Facts.OrderItem (
    OrderId bigint not null,
    OrderPositionId bigint not null,
    PricePositionId bigint null,
    ItemPositionId bigint not null,
    PackagePositionId bigint not null,
    FirmAddressId bigint null,
    CategoryId bigint null
)
go

create table Facts.OrderPosition (
    Id bigint not null,
    OrderId bigint not null,
    PricePositionId bigint not null,
    constraint PK_OrderPosition primary key (Id)
)
go
create index IX_OrderPosition_OrderId ON Facts.OrderPosition (OrderId) INCLUDE ([Id])
create index IX_OrderPosition_PricePositionId ON Facts.OrderPosition (PricePositionId) INCLUDE ([Id],[OrderId])
GO

create table Facts.OrderPositionAdvertisement (
    Id bigint not null,
    OrderPositionId bigint not null,
    PositionId bigint not null,
    FirmAddressId bigint null,
    CategoryId bigint null,
    AdvertisementId bigint null,
    ThemeId bigint null,

    constraint PK_OrderPositionAdvertisement primary key (Id)
)
go
CREATE INDEX IX_OrderPositionAdvertisement_AdvertisementId ON Facts.OrderPositionAdvertisement ([AdvertisementId]) INCLUDE ([OrderPositionId],[PositionId])
CREATE INDEX IX_OrderPositionAdvertisement_OrderPositionId ON Facts.OrderPositionAdvertisement ([OrderPositionId]) INCLUDE ([FirmAddressId],[PositionId])
create index IX_OrderPositionAdvertisement_PositionId ON Facts.OrderPositionAdvertisement (PositionId)
CREATE INDEX IX_OrderPositionAdvertisement_FirmAddressId_CategoryId ON [Facts].[OrderPositionAdvertisement] ([FirmAddressId],[CategoryId]) INCLUDE ([OrderPositionId],[PositionId])
CREATE INDEX IX_OrderPositionAdvertisement_CategoryId ON [Facts].[OrderPositionAdvertisement] ([CategoryId]) INCLUDE ([OrderPositionId])
GO

create table Facts.OrderPositionCostPerClick(
    OrderPositionId bigint not null,
    CategoryId bigint not null,
    Amount decimal(19,4) not null,
)
go

create table Facts.OrderScanFile (
    Id bigint not null,
    OrderId bigint not null,
    constraint PK_OrderScanFile primary key (Id)
)
go
CREATE INDEX IX_OrderScanFile_OrderId ON Facts.[OrderScanFile] ([OrderId]) INCLUDE ([Id])

create table Facts.Position(
    Id bigint not null,
    AdvertisementTemplateId bigint null,
    BindingObjectType int not null,
    SalesModel int not null,
    PositionsGroup int not null,
    IsCompositionOptional bit not null,
    IsControlledByAmount bit not null,
    IsComposite bit not null,
    CategoryCode bigint not null,
    [Platform] int not null,
    IsDeleted bit not null,
    constraint PK_Position primary key (Id)
)
create index IX_Position_IsComposite ON Facts.Position (IsComposite)
go

create table Facts.PositionChild(
    MasterPositionId bigint null,
    ChildPositionId bigint null,
)
go

create table Facts.Price(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    BeginDate datetime2(2) not null,
    constraint PK_Price primary key (Id)
)
go

create table Facts.PricePosition(
    Id bigint not null,
    PriceId bigint not null,
    PositionId bigint not null,
    MinAdvertisementAmount int null,
    MaxAdvertisementAmount int null,
    IsActiveNotDeleted bit not null,
    constraint PK_PricePosition primary key (Id)
)
create index IX_PricePosition_PriceId ON Facts.PricePosition (PriceId)
create index IX_PricePosition_PositionId ON Facts.PricePosition (PositionId)
go

create table Facts.Project(
    Id bigint not null,
    OrganizationUnitId bigint not null,
)
go

create table Facts.ReleaseInfo(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    PeriodEndDate datetime2(2) not null,
)
go

create table Facts.ReleaseWithdrawal(
    OrderPositionId bigint not null,
    [Start] datetime2(2) not null,
    Amount decimal(19,4) not null,
    constraint PK_ReleaseWithdrawal primary key (OrderPositionId, [Start])
)
go
CREATE NONCLUSTERED INDEX IX_ReleaseWithdrawal_OrderPositionId ON [Facts].[ReleaseWithdrawal] ([OrderPositionId]) INCLUDE ([Amount])
GO

create table Facts.RulesetRule(
    RuleType int not null,
    DependentPositionId bigint not null,
    PrincipalPositionId bigint not null,
    ObjectBindingType int not null
    constraint PK_RulesetRule primary key (RuleType, DependentPositionId, PrincipalPositionId)
)
go

create table Facts.SalesModelCategoryRestriction(
    ProjectId bigint not null,
    CategoryId bigint not null,
    [Begin] datetime2(2) not null,
    SalesModel int not null,
)
go

create table Facts.Theme (
    Id bigint not null,

    BeginDistribution datetime2(2) not null,
    EndDistribution datetime2(2) not null,
    IsDefault bit not null,

    constraint PK_Theme primary key (Id)
)
go

create table Facts.ThemeCategory (
    Id bigint not null,
    ThemeId bigint not null,
    CategoryId bigint not null,
    constraint PK_ThemeCategory primary key (Id)
)
go

create table Facts.ThemeOrganizationUnit (
    Id bigint not null,

    ThemeId bigint not null,
    OrganizationUnitId bigint not null,

    constraint PK_ThemeOrganizationUnit primary key (Id)
)
go

create table Facts.UnlimitedOrder(
    OrderId bigint not null,
    PeriodStart datetime2(2) not null,
    PeriodEnd datetime2(2) not null,
    constraint PK_UnlimitedOrder primary key (OrderId, PeriodStart)
)
go