if not exists (select * from sys.schemas where name = 'PriceAggregates') exec('create schema PriceAggregates')

if object_id('PriceAggregates.Period') is not null drop table PriceAggregates.Period

if object_id('PriceAggregates.OrderPeriod') is not null drop table PriceAggregates.OrderPeriod
if object_id('PriceAggregates.OrderPrice') is not null drop table PriceAggregates.OrderPrice
if object_id('PriceAggregates.OrderPricePosition') is not null drop table PriceAggregates.OrderPricePosition
if object_id('PriceAggregates.OrderCategoryPosition') is not null drop table PriceAggregates.OrderCategoryPosition
if object_id('PriceAggregates.OrderThemePosition') is not null drop table PriceAggregates.OrderThemePosition
if object_id('PriceAggregates.AmountControlledPosition') is not null drop table PriceAggregates.AmountControlledPosition
if object_id('PriceAggregates.ActualPrice') is not null drop table PriceAggregates.ActualPrice
if object_id('PriceAggregates.[Order]') is not null drop table PriceAggregates.[Order]

if object_id('PriceAggregates.Price') is not null drop table PriceAggregates.Price
if object_id('PriceAggregates.PricePeriod') is not null drop table PriceAggregates.PricePeriod
if object_id('PriceAggregates.PriceAssociatedPosition') is not null drop table PriceAggregates.PriceAssociatedPosition
if object_id('PriceAggregates.AdvertisementAmountRestriction') is not null drop table PriceAggregates.AdvertisementAmountRestriction
if object_id('PriceAggregates.AssociatedPositionGroupOvercount') is not null drop table PriceAggregates.AssociatedPositionGroupOvercount

if object_id('PriceAggregates.Firm') is not null drop table PriceAggregates.Firm
if object_id('PriceAggregates.FirmPosition') is not null drop table PriceAggregates.FirmPosition
if object_id('PriceAggregates.FirmAssociatedPosition') is not null drop table PriceAggregates.FirmAssociatedPosition
if object_id('PriceAggregates.FirmDeniedPosition') is not null drop table PriceAggregates.FirmDeniedPosition

go

-- price aggregate

create table PriceAggregates.Price(
    Id bigint NOT NULL,
    constraint PK_Price primary key (Id)
)
go

create table PriceAggregates.PricePeriod(
    PriceId bigint NOT NULL,
    ProjectId bigint NOT NULL,
    [Begin] datetime2(2) NOT NULL,
    [End] datetime2(2) NOT NULL,
)
go

create table PriceAggregates.AssociatedPositionGroupOvercount(
    PriceId bigint NOT NULL,
    PricePositionId bigint NOT NULL,
    PositionId bigint NOT NULL,
    [Count] int NOT NULL,
)
go

create table PriceAggregates.AdvertisementAmountRestriction(
    PriceId bigint NOT NULL,
    CategoryCode bigint NOT NULL,
    CategoryName nvarchar(128) NOT NULL,
    [Min] int NOT NULL,
    [Max] int NOT NULL,
    MissingMinimalRestriction bit NOT NULL,
)
create index IX_AdvertisementAmountRestriction_PriceId ON PriceAggregates.AdvertisementAmountRestriction (PriceId)
go

-- order aggregate
create table PriceAggregates.[Order](
    Id bigint NOT NULL,
    BeginDistribution datetime2(2) NOT NULL,
    EndDistributionPlan datetime2(2) NOT NULL,
    IsCommitted bit NOT NULL,
    constraint PK_Order primary key (Id)
)
go

create table PriceAggregates.OrderPeriod(
    OrderId bigint NOT NULL,
    [Begin] datetime2(2) NOT NULL,
    [End] datetime2(2) NOT NULL,
    Scope bigint NOT NULL,
)
go
create index IX_OrderPeriod_OrderId ON [PriceAggregates].[OrderPeriod] ([OrderId]) INCLUDE ([Begin],[End],[Scope]) -- todo: добавить в схему
GO

create table PriceAggregates.AmountControlledPosition(
    OrderId bigint NOT NULL,
    ProjectId bigint NOT NULL,
    CategoryCode bigint NOT NULL
)
go

create table PriceAggregates.ActualPrice(
    OrderId bigint NOT NULL,
    PriceId bigint NULL
)
go

create table PriceAggregates.OrderPricePosition(
    OrderId bigint NOT NULL,
	OrderPositionId bigint NOT NULL,
    PositionId bigint NOT NULL,
	PriceId bigint NOT NULL,
	IsActive bit NOT NULL
)
create index IX_OrderPricePosition_OrderId ON PriceAggregates.OrderPricePosition (OrderId)
create index IX_OrderPricePosition_PriceId ON PriceAggregates.OrderPricePosition (PriceId)
create index IX_OrderPricePosition_IsActive ON PriceAggregates.OrderPricePosition (IsActive)
go

create table PriceAggregates.OrderCategoryPosition(
    OrderId bigint NOT NULL,
    ProjectId bigint NOT NULL,
    OrderPositionAdvertisementId bigint NOT NULL,
    CategoryId bigint NOT NULL,
)
go

create table PriceAggregates.OrderThemePosition(
    OrderId bigint NOT NULL,
    ProjectId bigint NOT NULL,
    OrderPositionAdvertisementId bigint NOT NULL,
    ThemeId bigint NOT NULL,
)
go

-- period aggregate
create table PriceAggregates.Period(
    ProjectId bigint NOT NULL,
    OrganizationUnitId bigint NOT NULL,
    [Start] datetime2(2) NOT NULL,
    [End] datetime2(2) NOT NULL,
    constraint PK_Period primary key (OrganizationUnitId, [Start])
)
go

-- firm aggregate
create table PriceAggregates.Firm(
    Id bigint NOT NULL,
    constraint PK_Firm primary key (Id)
)
go

create table PriceAggregates.FirmPosition(
    FirmId bigint not null,
    OrderId bigint not null,
    OrderPositionId bigint not null,
    PackagePositionId bigint not null,
    ItemPositionId bigint not null,
    HasNoBinding bit not null,
    Category1Id bigint null,
    Category3Id bigint null,
    FirmAddressId bigint null,
    Scope bigint not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
)
go

create table PriceAggregates.FirmAssociatedPosition(
    FirmId bigint not null,
    OrderId bigint not null,
    OrderPositionId bigint not null,
    PackagePositionId bigint not null,
    ItemPositionId bigint not null,
    PrincipalPositionId bigint not null,
    BindingType int not null,
    [Source] int not null,
)
go

create table PriceAggregates.FirmDeniedPosition(
    FirmId bigint not null,
    OrderId bigint not null,
    OrderPositionId bigint not null,
    PackagePositionId bigint not null,
    ItemPositionId bigint not null,
    DeniedPositionId bigint not null,
    BindingType int not null,
    [Source] int not null,
)
go

CREATE NONCLUSTERED INDEX IX_FirmPosition_FirmId_ItemPositionId_Begin
ON [PriceAggregates].[FirmPosition] ([FirmId],[ItemPositionId],[Begin])
INCLUDE ([OrderId],[OrderPositionId],[PackagePositionId],[HasNoBinding],[Category1Id],[Category3Id],[FirmAddressId],[Scope],[End])
GO

CREATE NONCLUSTERED INDEX IX_FirmAssociatedPosition_OrderPositionId_ItemPositionId_FirmId
ON [PriceAggregates].FirmAssociatedPosition ([FirmId],[OrderPositionId],[ItemPositionId])
include([PrincipalPositionId],[BindingType])
GO

CREATE NONCLUSTERED INDEX IX_FirmDeniedPosition_FirmId_OrderPositionId_ItemPositionId
ON [PriceAggregates].[FirmDeniedPosition] ([FirmId],[OrderPositionId],[ItemPositionId])
INCLUDE ([DeniedPositionId],[BindingType])
GO
