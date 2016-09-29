if not exists (select * from sys.schemas where name = 'AdvertisementAggregates') exec('create schema AdvertisementAggregates')
go

if object_id('AdvertisementAggregates.Order') is not null drop table AdvertisementAggregates.[Order]
if object_id('AdvertisementAggregates.LinkedProject') is not null drop table AdvertisementAggregates.LinkedProject
if object_id('AdvertisementAggregates.RequiredAdvertisementMissing') is not null drop table AdvertisementAggregates.RequiredAdvertisementMissing
if object_id('AdvertisementAggregates.RequiredLinkedObjectCompositeMissing') is not null drop table AdvertisementAggregates.RequiredLinkedObjectCompositeMissing
if object_id('AdvertisementAggregates.AdvertisementDeleted') is not null drop table AdvertisementAggregates.AdvertisementDeleted
if object_id('AdvertisementAggregates.AdvertisementMustBelongToFirm') is not null drop table AdvertisementAggregates.AdvertisementMustBelongToFirm
if object_id('AdvertisementAggregates.AdvertisementIsDummy') is not null drop table AdvertisementAggregates.AdvertisementIsDummy
if object_id('AdvertisementAggregates.OrderAdvertisement') is not null drop table AdvertisementAggregates.OrderAdvertisement

if object_id('AdvertisementAggregates.Advertisement') is not null drop table AdvertisementAggregates.Advertisement
if object_id('AdvertisementAggregates.RequiredElementMissing') is not null drop table AdvertisementAggregates.RequiredElementMissing
if object_id('AdvertisementAggregates.ElementNotPassedReview') is not null drop table AdvertisementAggregates.ElementNotPassedReview

if object_id('AdvertisementAggregates.AdvertisementElementTemplate') is not null drop table AdvertisementAggregates.AdvertisementElementTemplate

if object_id('AdvertisementAggregates.Firm') is not null drop table AdvertisementAggregates.Firm
if object_id('AdvertisementAggregates.WhiteListDistributionPeriod') is not null drop table AdvertisementAggregates.WhiteListDistributionPeriod

if object_id('AdvertisementAggregates.Position') is not null drop table AdvertisementAggregates.Position
go

-- Order aggregate

create table AdvertisementAggregates.[Order] (
    Id bigint not null,
    Number nvarchar(64) not null,

    BeginDistributionDate datetime2(2) not null,
    EndDistributionDatePlan datetime2(2) not null,
    ProjectId bigint not null,
    FirmId bigint not null,
    RequireWhiteListAdvertisement bit not null,
    ProvideWhiteListAdvertisement bit not null,
)
go

create table AdvertisementAggregates.LinkedProject (
    OrderId bigint not null,
    ProjectId bigint not null,
)
go

create table AdvertisementAggregates.RequiredAdvertisementMissing (
    OrderId bigint not null,

    OrderPositionId bigint not null,
    CompositePositionId bigint not null,

    PositionId bigint not null,
)
go

create table AdvertisementAggregates.RequiredLinkedObjectCompositeMissing (
    OrderId bigint not null,

    OrderPositionId bigint not null,
    CompositePositionId bigint not null,

    PositionId bigint not null,
)
go

create table AdvertisementAggregates.AdvertisementDeleted (
    OrderId bigint not null,

    OrderPositionId bigint not null,
    PositionId bigint not null,

    AdvertisementId bigint not null,
    AdvertisementName nvarchar(128) not null,
)
go

create table AdvertisementAggregates.AdvertisementMustBelongToFirm (
    OrderId bigint not null,

    OrderPositionId bigint not null,
    PositionId bigint not null,

    AdvertisementId bigint not null,

    FirmId bigint not null,
)
go

create table AdvertisementAggregates.AdvertisementIsDummy (
    OrderId bigint not null,

    PositionId bigint not null,
)
go

create table AdvertisementAggregates.OrderAdvertisement (
    OrderId bigint not null,

    AdvertisementId bigint not null,
)
go

-- Advertisement aggregate

create table AdvertisementAggregates.Advertisement (
    Id bigint not null,
    Name nvarchar(128) not null,
    FirmId bigint not null,
    IsSelectedToWhiteList bit not null,
)
go

create table AdvertisementAggregates.RequiredElementMissing (
    AdvertisementId bigint not null,

    AdvertisementElementId bigint not null,

    AdvertisementElementTemplateId bigint not null,
)
go

create table AdvertisementAggregates.ElementNotPassedReview (
    AdvertisementId bigint not null,
    AdvertisementElementId bigint not null,
    AdvertisementElementTemplateId bigint not null,
    [Status] int not null,
)
go

-- AdvertisementElementTemplate aggregate

create table AdvertisementAggregates.AdvertisementElementTemplate (
    Id bigint not null,

    Name nvarchar(128) not null,
)
go


-- Firm aggregate

create table AdvertisementAggregates.Firm (
    Id bigint not null,
    Name nvarchar(250) not null,
)
go

create table AdvertisementAggregates.WhiteListDistributionPeriod (
    FirmId bigint not null,
    [Start] datetime2(2) not null,
    [End] datetime2(2) not null,
    AdvertisementId bigint null,
    ProvidedByOrderId bigint null,
)
go

-- Position aggregate

create table AdvertisementAggregates.Position (
    Id bigint not null,

    Name nvarchar(256) not null,
)
go
