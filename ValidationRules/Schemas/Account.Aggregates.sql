if not exists (select * from sys.schemas where name = 'AccountAggregate') exec('create schema AccountAggregate')

if object_id('AccountAggregate.Order') is not null drop table AccountAggregate.[Order]
if object_id('AccountAggregate.Lock') is not null drop table AccountAggregate.Lock
if object_id('AccountAggregate.Account') is not null drop table AccountAggregate.Account
if object_id('AccountAggregate.AccountPeriod') is not null drop table AccountAggregate.AccountPeriod
go

create table AccountAggregate.[Order](
    Id bigint not null,
    DestProjectId bigint not null,
    SourceProjectId bigint not null,
    AccountId bigint null,
    Number nvarchar(max) not null,
    BeginDistributionDate datetime2(2) not null,
    EndDistributionDate datetime2(2) not null,
    constraint PK_Order primary key (Id)
)
go

create table AccountAggregate.Lock(
    OrderId bigint not null,
    [Start] datetime2(2) not null,
    [End] datetime2(2) not null,
)
create index IX_Lock_OrderId ON AccountAggregate.Lock (OrderId)
go

create table AccountAggregate.Account(
    Id bigint not null,
    constraint PK_Account primary key (Id)
)
go

create table AccountAggregate.AccountPeriod(
    AccountId bigint not null,
    Balance decimal(19,4) not null,
    LockedAmount decimal(19,4) not null,
    OwerallLockedAmount decimal(19,4) not null,
    ReleaseAmount decimal(19,4) not null,
    LimitAmount decimal(19,4) not null,

    [Start] datetime2(2) not null,
    [End] datetime2(2) not null,

)
--create index IX_AccountPeriod_ ON AccountAggregate.Lock (AccountId)
go
