if not exists (select * from sys.schemas where name = 'AccountAggregate') exec('create schema AccountAggregate')

if object_id('AccountAggregate.Order') is not null drop table AccountAggregate.[Order]
if object_id('AccountAggregate.Lock') is not null drop table AccountAggregate.Lock
go

create table AccountAggregate.[Order](
    Id bigint not null,
    ProjectId bigint not null,
    Number nvarchar(max) not null,
    HasAccount bit not null,
    BeginDistributionDate datetime2(2) not null,
    EndDistributionDatePlan datetime2(2) not null,
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
