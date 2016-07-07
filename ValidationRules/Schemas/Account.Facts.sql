if not exists (select * from sys.schemas where name = 'AccountContext') exec('create schema AccountContext')
go

if object_id('AccountContext.Order') is not null drop table AccountContext.[Order]
if object_id('AccountContext.Project') is not null drop table AccountContext.Project
if object_id('AccountContext.Account') is not null drop table AccountContext.Account
if object_id('AccountContext.Lock') is not null drop table AccountContext.Lock
if object_id('AccountContext.Limit') is not null drop table AccountContext.Limit
if object_id('AccountContext.ReleaseWithdrawal') is not null drop table AccountContext.ReleaseWithdrawal
if object_id('AccountContext.OrderPosition') is not null drop table AccountContext.OrderPosition
go

create table AccountContext.[Order](
    Id bigint not null,
    DestOrganizationUnitId bigint not null,
    SourceOrganizationUnitId bigint not null,
    AccountId bigint null,
    Number nvarchar(64) not null,
    BeginDistributionDate datetime2(2) not null,
    EndDistributionDate datetime2(2) not null,
    constraint PK_Order primary key (Id)
)
go

create table AccountContext.Project(
    Id bigint not null,
    OrganizationUnitId bigint null,
    constraint PK_Project primary key (Id)
)
go

create table AccountContext.Account(
    Id bigint not null,
    Balance decimal(19,4) not null,
    constraint PK_Account primary key (Id)
)
go

create table AccountContext.Lock(
    Id bigint not null,
    OrderId bigint not null,
    AccountId bigint not null,
    Start datetime2(2) not null,
    Amount decimal(19,4) not null,
    constraint PK_Lock primary key (Id)
)
go

create table AccountContext.Limit(
    Id bigint not null,
    AccountId bigint not null,
    Start datetime2(2) not null,
    Amount decimal(19,4) not null,
    constraint PK_Limit primary key (Id)
)
go

create table AccountContext.ReleaseWithdrawal(
    Id bigint not null,
    OrderPositionId bigint not null,
    Start datetime2(2) not null,
    Amount decimal(19,4) not null,
    constraint PK_ReleaseWithdrawal primary key (Id)
)
go

create table AccountContext.OrderPosition(
    Id bigint not null,
    OrderId bigint not null,
    constraint PK_OrderPosition primary key (Id)
)
go
