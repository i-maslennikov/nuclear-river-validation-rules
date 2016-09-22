if not exists (select * from sys.schemas where name = 'FirmAggregates') exec('create schema FirmAggregates')

if object_id('FirmAggregates.Order') is not null drop table FirmAggregates.[Order]
if object_id('FirmAggregates.Firm') is not null drop table FirmAggregates.Firm
if object_id('FirmAggregates.FirmOrganiationUnitMismatch') is not null drop table FirmAggregates.FirmOrganiationUnitMismatch
if object_id('FirmAggregates.CategoryPurchase') is not null drop table FirmAggregates.CategoryPurchase
if object_id('FirmAggregates.SpecialPosition') is not null drop table FirmAggregates.SpecialPosition
go

create table FirmAggregates.[Order](
    Id bigint not null,
    ProjectId bigint not null,
    FirmId bigint not null,
    Number nvarchar(64) not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
    Scope bigint not null,
    constraint PK_Order primary key (Id)
)
go

create table FirmAggregates.Firm(
    Id bigint not null,
    Name nvarchar(250) not null,
    NeedsSpecialPosition bit not null,
    constraint PK_Firm primary key (Id)
)
go

create table FirmAggregates.FirmOrganiationUnitMismatch(
    OrderId bigint not null,
)
go

create table FirmAggregates.CategoryPurchase(
    OrderId bigint not null,
    CategoryId bigint not null,
)
go

create table FirmAggregates.SpecialPosition(
    OrderId bigint not null,
)
go

CREATE NONCLUSTERED INDEX IX_CategoryPurchase_OrderId
ON [FirmAggregates].[CategoryPurchase] ([OrderId])
INCLUDE ([CategoryId])
GO
