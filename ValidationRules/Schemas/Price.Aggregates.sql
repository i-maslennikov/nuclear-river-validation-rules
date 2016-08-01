if not exists (select * from sys.schemas where name = 'PriceAggregates') exec('create schema PriceAggregates')

if object_id('PriceAggregates.Position') is not null drop table PriceAggregates.Position

if object_id('PriceAggregates.OrderPeriod') is not null drop table PriceAggregates.OrderPeriod
if object_id('PriceAggregates.PricePeriod') is not null drop table PriceAggregates.PricePeriod
if object_id('PriceAggregates.Period') is not null drop table PriceAggregates.Period

if object_id('PriceAggregates.OrderPrice') is not null drop table PriceAggregates.OrderPrice
if object_id('PriceAggregates.OrderPricePosition') is not null drop table PriceAggregates.OrderPricePosition
if object_id('PriceAggregates.OrderPosition') is not null drop table PriceAggregates.OrderPosition
if object_id('PriceAggregates.OrderDeniedPosition') is not null drop table PriceAggregates.OrderDeniedPosition
if object_id('PriceAggregates.OrderAssociatedPosition') is not null drop table PriceAggregates.OrderAssociatedPosition
if object_id('PriceAggregates.AmountControlledPosition') is not null drop table PriceAggregates.AmountControlledPosition
if object_id('PriceAggregates.[Order]') is not null drop table PriceAggregates.[Order]

if object_id('PriceAggregates.PriceAssociatedPosition') is not null drop table PriceAggregates.PriceAssociatedPosition
if object_id('PriceAggregates.AdvertisementAmountRestriction') is not null drop table PriceAggregates.AdvertisementAmountRestriction
if object_id('PriceAggregates.Price') is not null drop table PriceAggregates.Price
if object_id('PriceAggregates.AssociatedPositionGroupOvercount') is not null drop table PriceAggregates.AssociatedPositionGroupOvercount

if object_id('PriceAggregates.Project') is not null drop table PriceAggregates.Project

go

-- справочник
create table PriceAggregates.Project(
    Id bigint NOT NULL,
    Name nvarchar(max) NOT NULL,
    constraint PK_Project primary key (Id)
)
go

create table PriceAggregate.Theme(
    Id bigint NOT NULL,
    Name nvarchar(max) NOT NULL,
    constraint PK_Theme primary key (Id)
)
go

-- price aggregate

create table PriceAggregates.Price(
    Id bigint NOT NULL,
    BeginDate datetime2(2) NOT NULL,
    constraint PK_Price primary key (Id)
)
go

create table PriceAggregates.AssociatedPositionGroupOvercount(
    PriceId bigint NOT NULL,
    PricePositionId bigint NOT NULL,
    PricePositionName nvarchar(max) NOT NULL,
    Count int NOT NULL,
)
go

create table PriceAggregates.AdvertisementAmountRestriction(
    PriceId bigint NOT NULL,
    CategoryCode bigint NOT NULL,
    CategoryName nvarchar(max) NOT NULL,
    [Min] int NOT NULL,
    [Max] int NOT NULL,
    MissingMinimalRestriction bit NOT NULL,
)
create index IX_AdvertisementAmountRestriction_PriceId ON PriceAggregates.AdvertisementAmountRestriction (PriceId)
go

-- order aggregate
create table PriceAggregates.[Order](
    Id bigint NOT NULL,
    FirmId bigint NOT NULL,
    Number nvarchar(max) NOT NULL,
    constraint PK_Order primary key (Id)
)
create index IX_Order_FirmId ON PriceAggregates.[Order] ([FirmId]) include (Id)
go

create table PriceAggregates.AmountControlledPosition(
    OrderId bigint NOT NULL,
    CategoryCode bigint NOT NULL
)
go

create table PriceAggregates.OrderPosition(
    OrderId bigint NOT NULL,
    OrderPositionId bigint NOT NULL,
    PackagePositionId bigint NOT NULL,
    ItemPositionId bigint NOT NULL,

    HasNoBinding bit NOT NULL,
    Category3Id bigint NULL,
    Category1Id bigint NULL,
    FirmAddressId bigint NULL,

    ThemeId bigint NULL,

    Source nvarchar(16) NULL,
)
create index IX_OrderPosition_OrderId ON PriceAggregates.OrderPosition (OrderId)
go

create table PriceAggregates.OrderDeniedPosition(
    OrderId bigint NOT NULL,
    CauseOrderPositionId bigint NOT NULL,
    CausePackagePositionId bigint NOT NULL,
    CauseItemPositionId bigint NOT NULL,

    DeniedPositionId bigint NOT NULL,
    BindingType int NOT NULL,

	HasNoBinding bit NOT NULL,
    Category3Id bigint NULL,
    Category1Id bigint NULL,
    FirmAddressId bigint NULL,

    Source nvarchar(16) not null,
)
create index IX_OrderDeniedPosition_OrderId_DeniedPositionId_BindingType
on [PriceAggregates].[OrderDeniedPosition] ([OrderId],[DeniedPositionId],[BindingType])
include ([CauseOrderPositionId],[CausePackagePositionId],[CauseItemPositionId],[HasNoBinding],[Category3Id],[Category1Id],[FirmAddressId])
go

create table PriceAggregates.OrderAssociatedPosition(
    OrderId bigint NOT NULL,
    CauseOrderPositionId bigint NOT NULL,
    CausePackagePositionId bigint NOT NULL,
    CauseItemPositionId bigint NOT NULL,

    PrincipalPositionId bigint NOT NULL,
	BindingType int NOT NULL,

	HasNoBinding bit NOT NULL,
    Category3Id bigint NULL,
    Category1Id bigint NULL,
    FirmAddressId bigint NULL,

	Source nvarchar(16) not null,
)
create index IX_OrderAssociatedPosition_OrderId_PrincipalPositionId_BindingType
on [PriceAggregates].[OrderAssociatedPosition] ([OrderId],[PrincipalPositionId],[BindingType])
include ([CauseOrderPositionId],[CausePackagePositionId],[CauseItemPositionId],[HasNoBinding],[Category3Id],[Category1Id],[FirmAddressId])

create index IX_OrderAssociatedPosition_CauseOrderPositionId_CauseItemPositionId_BindingType
on [PriceAggregates].[OrderAssociatedPosition] ([CauseOrderPositionId],[CauseItemPositionId],[BindingType])
go

create table PriceAggregates.OrderPricePosition(
    OrderId bigint NOT NULL,
	OrderPositionId bigint NOT NULL,
	OrderPositionName nvarchar(max) NULL,
	PriceId bigint NOT NULL,
	IsActive bit NOT NULL
)
create index IX_OrderPricePosition_OrderId ON PriceAggregates.OrderPricePosition (OrderId)
create index IX_OrderPricePosition_PriceId ON PriceAggregates.OrderPricePosition (PriceId)
create index IX_OrderPricePosition_IsActive ON PriceAggregates.OrderPricePosition (IsActive)
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

create table PriceAggregates.OrderPeriod(
    OrderId bigint NOT NULL,
    OrganizationUnitId bigint NOT NULL,
    Start datetime2(2) NOT NULL,
    Scope bigint NOT NULL,
)
create index IX_OrderPeriod_OrderId ON PriceAggregates.OrderPeriod (OrderId)
create index IX_OrderPeriod_OrganizationUnitId_Start ON PriceAggregates.OrderPeriod (OrganizationUnitId, Start)
go

create table PriceAggregates.PricePeriod(
    PriceId bigint NOT NULL,
    OrganizationUnitId bigint NOT NULL,
    Start datetime2(2) NOT NULL,
)
create index IX_PricePeriod_PriceId ON PriceAggregates.PricePeriod (PriceId)
create index IX_PricePeriod_OrganizationUnitId_Start ON PriceAggregates.PricePeriod (OrganizationUnitId, Start)
go

-- position aggregate
create table PriceAggregates.Position(
    Id bigint NOT NULL,
    CategoryCode bigint NOT NULL,
    IsControlledByAmount bit NOT NULL,
    Name nvarchar(max) NOT NULL,
    constraint PK_Position primary key (Id)
)
go
