if not exists (select * from sys.schemas where name = 'AccountFacts') exec('create schema AccountFacts')
go

if object_id('AccountFacts.Order') is not null drop table AccountFacts.[Order]
if object_id('AccountFacts.Project') is not null drop table AccountFacts.Project
if object_id('AccountFacts.Account') is not null drop table AccountFacts.Account
if object_id('AccountFacts.Lock') is not null drop table AccountFacts.Lock
if object_id('AccountFacts.ReleaseWithdrawal') is not null drop table AccountFacts.ReleaseWithdrawal
if object_id('AccountFacts.OrderPosition') is not null drop table AccountFacts.OrderPosition
if object_id('AccountFacts.UnlimitedOrder') is not null drop table AccountFacts.UnlimitedOrder
go

create table AccountFacts.[Order](
    Id bigint not null,
    DestOrganizationUnitId bigint not null,
    SourceOrganizationUnitId bigint not null,
    BranchOfficeOrganizationUnitId bigint null,
    LegalPersonId bigint null,
    IsFreeOfCharge bit not null,
    Number nvarchar(64) not null,
    BeginDistributionDate datetime2(2) not null,
    EndDistributionDate datetime2(2) not null,
    constraint PK_Order primary key (Id)
)
go

create table AccountFacts.Project(
    Id bigint not null,
    OrganizationUnitId bigint null,
    constraint PK_Project primary key (Id)
)
go

create table AccountFacts.Account(
    Id bigint not null,
    BranchOfficeOrganizationUnitId bigint not null,
    LegalPersonId bigint not null,
    Balance decimal(19,4) not null,
    constraint PK_Account primary key (Id)
)
go

create table AccountFacts.Lock(
    Id bigint not null,
    OrderId bigint not null,
    AccountId bigint not null,
    Start datetime2(2) not null,
    Amount decimal(19,4) not null,
    constraint PK_Lock primary key (Id)
)
go

create table AccountFacts.UnlimitedOrder(
    OrderId bigint not null,
    PeriodStart datetime2(2) not null,
    PeriodEnd datetime2(2) not null,
    constraint PK_UnlimitedOrder primary key (OrderId, PeriodStart)
)
go

create table AccountFacts.ReleaseWithdrawal(
    Id bigint not null,
    OrderPositionId bigint not null,
    Start datetime2(2) not null,
    Amount decimal(19,4) not null,
    constraint PK_ReleaseWithdrawal primary key (Id)
)
go

create table AccountFacts.OrderPosition(
    Id bigint not null,
    OrderId bigint not null,
    constraint PK_OrderPosition primary key (Id)
)
go

CREATE NONCLUSTERED INDEX IX_Order_SourceOrganizationUnitId
ON [AccountFacts].[Order] ([SourceOrganizationUnitId])
INCLUDE ([Id],[DestOrganizationUnitId],[BranchOfficeOrganizationUnitId],[LegalPersonId],[Number],[BeginDistributionDate],[EndDistributionDate])
GO
