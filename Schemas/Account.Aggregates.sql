if not exists (select * from sys.schemas where name = 'AccountAggregates') exec('create schema AccountAggregates')

if object_id('AccountAggregates.Order') is not null drop table AccountAggregates.[Order]
if object_id('AccountAggregates.Lock') is not null drop table AccountAggregates.Lock
if object_id('AccountAggregates.Account') is not null drop table AccountAggregates.Account
if object_id('AccountAggregates.AccountPeriod') is not null drop table AccountAggregates.AccountPeriod
go

create table AccountAggregates.[Order](
    Id bigint not null,
    DestProjectId bigint not null,
    SourceProjectId bigint not null,
    AccountId bigint null,
    IsFreeOfCharge bit not null,
    Number nvarchar(64) not null,
    BeginDistributionDate datetime2(2) not null,
    EndDistributionDate datetime2(2) not null,
    constraint PK_Order primary key (Id)
)
go

create table AccountAggregates.Lock(
    OrderId bigint not null,
    [Start] datetime2(2) not null,
    [End] datetime2(2) not null,
)
create index IX_Lock_OrderId ON AccountAggregates.Lock (OrderId)
go

create table AccountAggregates.Account(
    Id bigint not null,
    constraint PK_Account primary key (Id)
)
go

create table AccountAggregates.AccountPeriod(
    AccountId bigint not null,
    Balance decimal(19,4) not null,
    LockedAmount decimal(19,4) not null,
    OwerallLockedAmount decimal(19,4) not null,
    ReleaseAmount decimal(19,4) not null,
    LimitAmount decimal(19,4) not null,

    [Start] datetime2(2) not null,
    [End] datetime2(2) not null,

)
--create index IX_AccountPeriod_ ON AccountAggregates.Lock (AccountId)
go

CREATE NONCLUSTERED INDEX IX_AccountPeriod_AccountId_Start_End
ON [AccountAggregates].[AccountPeriod] ([AccountId],[Start],[End])
INCLUDE ([Balance],[LockedAmount],[OwerallLockedAmount],[ReleaseAmount],[LimitAmount])
GO
