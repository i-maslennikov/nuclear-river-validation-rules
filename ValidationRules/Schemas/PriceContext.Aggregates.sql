if not exists (select * from sys.schemas where name = 'PriceAggregate') exec('create schema PriceAggregate')

if object_id('PriceAggregate.Position') is not null drop table PriceAggregate.Position

if object_id('PriceAggregate.OrderPeriod') is not null drop table PriceAggregate.OrderPeriod
if object_id('PriceAggregate.PricePeriod') is not null drop table PriceAggregate.PricePeriod
if object_id('PriceAggregate.Period') is not null drop table PriceAggregate.Period

if object_id('PriceAggregate.OrderPrice') is not null drop table PriceAggregate.OrderPrice
if object_id('PriceAggregate.OrderPosition') is not null drop table PriceAggregate.OrderPosition
if object_id('PriceAggregate.[Order]') is not null drop table PriceAggregate.[Order]

if object_id('PriceAggregate.DeniedPosition') is not null drop table PriceAggregate.DeniedPosition
if object_id('PriceAggregate.MasterPosition') is not null drop table PriceAggregate.MasterPosition
if object_id('PriceAggregate.AdvertisementAmountRestriction') is not null drop table PriceAggregate.AdvertisementAmountRestriction
if object_id('PriceAggregate.Price') is not null drop table PriceAggregate.Price
go


-- price aggregate

create table PriceAggregate.Price(
    Id bigint NOT NULL,
    constraint PK_Price primary key (Id)
)
go

create table PriceAggregate.DeniedPosition(
    PositionId bigint NOT NULL,
    DeniedPositionId bigint NOT NULL,
    PriceId bigint NULL,
    ObjectBindingType int NOT NULL
)
create index IX_DeniedPosition_PriceId ON PriceAggregate.DeniedPosition (PriceId)
go

create table PriceAggregate.MasterPosition(
    PositionId bigint NOT NULL,
    MasterPositionId bigint NOT NULL,
    PriceId bigint NULL,
    GroupId bigint NULL,
    ObjectBindingType int NOT NULL
)
create index IX_MasterPosition_PriceId ON PriceAggregate.MasterPosition (PriceId)
go

create table PriceAggregate.AdvertisementAmountRestriction(
    PriceId bigint NOT NULL,
    PositionId bigint NOT NULL,
    [Min] int NOT NULL,
    [Max] int NOT NULL
)
create index IX_AdvertisementAmountRestriction_PriceId ON PriceAggregate.AdvertisementAmountRestriction (PriceId)
go

-- order aggregate
create table PriceAggregate.[Order](
    Id bigint NOT NULL,
    FirmId bigint NOT NULL,
    constraint PK_Order primary key (Id)
)
go

create table PriceAggregate.OrderPosition(
    OrderId bigint NOT NULL,
    CompareMode int NOT NULL,
    PackagePositionId bigint NOT NULL,
    ItemPositionId bigint NOT NULL,
    Category3Id bigint NULL,
    Category1Id bigint NULL,
    FirmAddressId bigint NULL
)
create index IX_OrderPosition_OrderId ON PriceAggregate.OrderPosition (OrderId)
go

create table PriceAggregate.OrderPrice(
    OrderId bigint NOT NULL,
    PriceId bigint NOT NULL
)
create index IX_OrderPrice_OrderId ON PriceAggregate.OrderPrice (OrderId)
go

-- period aggregate
create table PriceAggregate.Period(
    Id bigint NOT NULL,
    OrganizationUnitId bigint NOT NULL,
    Start datetime2(2) NOT NULL,
    [End] datetime2(2) NOT NULL,
    constraint PK_Period primary key (Id)
)
go

create table PriceAggregate.OrderPeriod(
    OrderId bigint NOT NULL,
    PeriodId bigint NOT NULL
)
create index IX_OrderPeriod_OrderId ON PriceAggregate.OrderPeriod (OrderId)
create index IX_OrderPeriod_PeriodId ON PriceAggregate.OrderPeriod (PeriodId)
go

create table PriceAggregate.PricePeriod(
    PriceId bigint NOT NULL,
    PeriodId bigint NOT NULL
)
create index IX_PricePeriod_PriceId ON PriceAggregate.PricePeriod (PriceId)
create index IX_PricePeriod_PeriodId ON PriceAggregate.PricePeriod (PeriodId)
go

-- position aggregate
create table PriceAggregate.Position(
    Id bigint NOT NULL,
    PositionCategoryId bigint NOT NULL,
    constraint PK_Position primary key (Id)
)
go