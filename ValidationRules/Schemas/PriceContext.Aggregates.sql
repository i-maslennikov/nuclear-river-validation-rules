if not exists (select * from sys.schemas where name = 'PriceAggregate') exec('create schema PriceAggregate')

if object_id('PriceAggregate.DeniedPosition') is not null drop table PriceAggregate.DeniedPosition
if object_id('PriceAggregate.MasterPosition') is not null drop table PriceAggregate.MasterPosition
if object_id('PriceAggregate.[Order]') is not null drop table PriceAggregate.[Order]
if object_id('PriceAggregate.OrderPeriod') is not null drop table PriceAggregate.OrderPeriod
if object_id('PriceAggregate.OrderPosition') is not null drop table PriceAggregate.OrderPosition
if object_id('PriceAggregate.OrderPrice') is not null drop table PriceAggregate.OrderPrice
if object_id('PriceAggregate.Period') is not null drop table PriceAggregate.Period
if object_id('PriceAggregate.Position') is not null drop table PriceAggregate.Position
if object_id('PriceAggregate.Price') is not null drop table PriceAggregate.Price
if object_id('PriceAggregate.PricePeriod') is not null drop table PriceAggregate.PricePeriod
if object_id('PriceAggregate.PricePosition') is not null drop table PriceAggregate.PricePosition
go

create table PriceAggregate.DeniedPosition(
    PositionId bigint NOT NULL,
    DeniedPositionId bigint NOT NULL,
    PriceId bigint NULL,
    ObjectBindingType int NOT NULL
)
go

create table PriceAggregate.MasterPosition(
    PositionId bigint NOT NULL,
    MasterPositionId bigint NOT NULL,
    PriceId bigint NULL,
    GroupId bigint NULL,
    ObjectBindingType int NOT NULL
)
go

create table PriceAggregate.[Order](
    Id bigint NOT NULL,
    FirmId bigint NOT NULL
)
go

create table PriceAggregate.OrderPeriod(
    OrderId bigint NOT NULL,
    PeriodId bigint NOT NULL
)
go

create table PriceAggregate.OrderPosition(
    OrderId bigint NOT NULL,
    PackagePositionId bigint NULL,
    ItemPositionId bigint NOT NULL,
    CategoryId bigint NULL,
    FirmAddressId bigint NULL,
)
go

create table PriceAggregate.OrderPrice(
    OrderId bigint NOT NULL,
    PriceId bigint NOT NULL
)
go

create table PriceAggregate.Period(
    Id bigint NOT NULL,
    OrganizationUnitId bigint NOT NULL,
    Start datetime2(2) NOT NULL,
    [End] datetime2(2) NOT NULL
)
go

create table PriceAggregate.Position(
    Id bigint NOT NULL,
    PositionCategoryId bigint NOT NULL
)
go

create table PriceAggregate.Price(
    Id bigint NOT NULL
)
go

create table PriceAggregate.PricePeriod(
    PriceId bigint NOT NULL,
    PeriodId bigint NOT NULL
)
go

create table PriceAggregate.AdvertisementAmountRestriction(
    PriceId bigint NOT NULL,
    PositionId bigint NOT NULL,
    [Min] int NOT NULL,
    [Max] int NOT NULL
)
go
