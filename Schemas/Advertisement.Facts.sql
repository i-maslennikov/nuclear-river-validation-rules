if not exists (select * from sys.schemas where name = 'AdvertisementFacts') exec('create schema AdvertisementFacts')
go

if object_id('AdvertisementFacts.AdvertisementTemplate') is not null drop table AdvertisementFacts.AdvertisementTemplate
if object_id('AdvertisementFacts.Position') is not null drop table AdvertisementFacts.Position
if object_id('AdvertisementFacts.PricePosition') is not null drop table AdvertisementFacts.PricePosition
if object_id('AdvertisementFacts.OrderPositionAdvertisement') is not null drop table AdvertisementFacts.OrderPositionAdvertisement
if object_id('AdvertisementFacts.OrderPosition') is not null drop table AdvertisementFacts.OrderPosition
if object_id('AdvertisementFacts.Order') is not null drop table AdvertisementFacts.[Order]
if object_id('AdvertisementFacts.Project') is not null drop table AdvertisementFacts.Project
if object_id('AdvertisementFacts.Advertisement') is not null drop table AdvertisementFacts.Advertisement
if object_id('AdvertisementFacts.Firm') is not null drop table AdvertisementFacts.Firm
if object_id('AdvertisementFacts.AdvertisementElement') is not null drop table AdvertisementFacts.AdvertisementElement
if object_id('AdvertisementFacts.AdvertisementElementTemplate') is not null drop table AdvertisementFacts.AdvertisementElementTemplate
go

create table AdvertisementFacts.AdvertisementTemplate (
    Id bigint not null,
    DummyAdvertisementId bigint not null,
    IsAdvertisementRequired bit not null,
    IsAllowedToWhiteList bit not null,
    constraint PK_AdvertisementTemplate primary key (Id)
)
go

create table AdvertisementFacts.Position (
    Id bigint not null,
    AdvertisementTemplateId bigint null,
    Name nvarchar(256) not null,

    IsCompositionOptional bit not null,

    ChildPositionId bigint null,
)
go

create table AdvertisementFacts.PricePosition (
    Id bigint not null,
    PositionId bigint not null,
    constraint PK_PricePosition primary key (Id)
)
go

create table AdvertisementFacts.OrderPositionAdvertisement (
    Id bigint not null,
    OrderPositionId bigint not null,
    PositionId bigint not null,
    AdvertisementId bigint null,
    constraint PK_OrderPositionAdvertisement primary key (Id)
)
go

-- 4 index
CREATE NONCLUSTERED INDEX IX_OrderPositionAdvertisement_OrderPositionId
ON [AdvertisementFacts].[OrderPositionAdvertisement] ([OrderPositionId])

-- 1 index
CREATE NONCLUSTERED INDEX IX_OrderPositionAdvertisement_AdvertisementId
ON [AdvertisementFacts].[OrderPositionAdvertisement] ([AdvertisementId])
INCLUDE ([OrderPositionId],[PositionId])


create table AdvertisementFacts.OrderPosition (
    Id bigint not null,
    OrderId bigint not null,
    PricePositionId bigint not null,
    constraint PK_OrderPosition primary key (Id)
)
go

-- 3 index
CREATE NONCLUSTERED INDEX IX_OrderPosition_OrderId
ON [AdvertisementFacts].[OrderPosition] ([OrderId])
INCLUDE ([Id])

-- 2 index
CREATE NONCLUSTERED INDEX IX_OrderPosition_PricePositionId
ON [AdvertisementFacts].[OrderPosition] ([PricePositionId])
INCLUDE ([Id],[OrderId])

create table AdvertisementFacts.[Order] (
    Id bigint not null,
    FirmId bigint not null,
    Number nvarchar(64) not null,

    BeginDistributionDate datetime2(2) not null,
    EndDistributionDatePlan datetime2(2) not null,
    EndDistributionDateFact datetime2(2) not null,
    DestOrganizationUnitId bigint not null,
    SourceOrganizationUnitId bigint not null,
    WorkflowStepId int not null,

    constraint PK_Order primary key (Id)
)
go

create table AdvertisementFacts.Project (
    Id bigint not null,
    OrganizationUnitId bigint not null,
)
go

create table AdvertisementFacts.Advertisement (
    Id bigint not null,
    FirmId bigint null,
    AdvertisementTemplateId bigint not null,
    Name nvarchar(128) not null,
    IsSelectedToWhiteList bit not null,
    IsDeleted bit not null,
    constraint PK_Advertisement primary key (Id)
)
go

-- 5 index
CREATE NONCLUSTERED INDEX IX_Advertisement_IsDeleted
ON [AdvertisementFacts].[Advertisement] ([IsDeleted])
INCLUDE ([Id])

create table AdvertisementFacts.Firm (
    Id bigint not null,
    Name nvarchar(250) not null,
    constraint PK_Firm primary key (Id)
)
go

create table AdvertisementFacts.AdvertisementElement (
    Id bigint not null,
    AdvertisementId bigint not null,
    AdvertisementElementTemplateId bigint not null,
    IsEmpty bit not null,
    [Status] int not null,
    constraint PK_AdvertisementElement primary key (Id)
)
go

-- 6 index
CREATE NONCLUSTERED INDEX IX_AdvertisementElement_AdvertisementElementTemplateId_IsEmpty
ON [AdvertisementFacts].[AdvertisementElement] ([AdvertisementElementTemplateId],[IsEmpty])
INCLUDE ([Id],[AdvertisementId])
-- 7 index
CREATE NONCLUSTERED INDEX IX_AdvertisementElement_AdvertisementElementTemplateId_Status
ON [AdvertisementFacts].[AdvertisementElement] (AdvertisementElementTemplateId,[Status])
INCLUDE ([Id],[AdvertisementId])


create table AdvertisementFacts.AdvertisementElementTemplate (
    Id bigint not null,
    Name nvarchar(128) not null,
    IsRequired bit not null,
    NeedsValidation bit not null,
    constraint PK_AdvertisementElementTemplate primary key (Id)
)
go