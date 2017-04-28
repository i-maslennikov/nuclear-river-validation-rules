if not exists (select * from sys.schemas where name = 'FirmAggregates') exec('create schema FirmAggregates')
go

if object_id('FirmAggregates.Order') is not null drop table FirmAggregates.[Order]
if object_id('FirmAggregates.InvalidFirm') is not null drop table FirmAggregates.InvalidFirm
if object_id('FirmAggregates.Firm') is not null drop table FirmAggregates.Firm
if object_id('FirmAggregates.AdvantageousPurchasePositionDistributionPeriod') is not null drop table FirmAggregates.AdvantageousPurchasePositionDistributionPeriod
if object_id('FirmAggregates.FirmOrganiationUnitMismatch') is not null drop table FirmAggregates.FirmOrganiationUnitMismatch
if object_id('FirmAggregates.CategoryPurchase') is not null drop table FirmAggregates.CategoryPurchase
if object_id('FirmAggregates.NotApplicapleForDesktopPosition') is not null drop table FirmAggregates.NotApplicapleForDesktopPosition
if object_id('FirmAggregates.SelfAdvertisementPosition') is not null drop table FirmAggregates.SelfAdvertisementPosition
go

create table FirmAggregates.[Order](
    Id bigint not null,
    FirmId bigint not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
    Scope bigint not null,
    constraint PK_Order primary key (Id)
)
create index IX_Order_FirmId_Begin_End on FirmAggregates.[Order] (FirmId, [Begin], [End]) include ([Id], [Scope])

create table FirmAggregates.InvalidFirm(
    OrderId bigint not null,
    FirmId bigint not null,
    [State] int not null,
)

create table FirmAggregates.Firm(
    Id bigint not null,
    ProjectId bigint not null,
    constraint PK_Firm primary key (Id)
)

create table FirmAggregates.AdvantageousPurchasePositionDistributionPeriod(
    FirmId bigint not null,
    Scope bigint not null,
    HasPosition bit not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
)

create table FirmAggregates.FirmOrganiationUnitMismatch(
    OrderId bigint not null,
)

create table FirmAggregates.CategoryPurchase(
    FirmId bigint not null,
    CategoryId bigint not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
    Scope bigint not null,
)
create index IX_CategoryPurchase_FirmId_Begin_End_CategoryId on FirmAggregates.CategoryPurchase (FirmId, [Begin], [End], [CategoryId]) include ([Scope])

create table FirmAggregates.NotApplicapleForDesktopPosition(
    OrderId bigint not null,
)

create table FirmAggregates.SelfAdvertisementPosition(
    OrderId bigint not null,
)
go
