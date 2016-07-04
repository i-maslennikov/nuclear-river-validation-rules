if not exists (select * from sys.schemas where name = 'AccountContext') exec('create schema AccountContext')
go

if object_id('AccountContext.Order') is not null drop table AccountContext.[Order]
if object_id('AccountContext.Project') is not null drop table AccountContext.Project
if object_id('AccountContext.Account') is not null drop table AccountContext.Account
if object_id('AccountContext.Lock') is not null drop table AccountContext.Lock
go

create table AccountContext.[Order](
    Id bigint not null,
    DestOrganizationUnitId bigint not null,
    AccountId bigint null,
    Number nvarchar(64) not null,
    BeginDistributionDate datetime2(2) not null,
    EndDistributionDatePlan datetime2(2) not null,
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
    constraint PK_Account primary key (Id)
)
go

create table AccountContext.Lock(
    Id bigint not null,
    OrderId bigint not null,
    PeriodStartDate datetime2(2) not null,
    PeriodEndDate datetime2(2) not null,
    constraint PK_Lock primary key (Id)
)
go
