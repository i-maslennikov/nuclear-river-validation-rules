if not exists (select * from sys.schemas where name = 'AccountAggregate') exec('create schema AccountAggregate')

if object_id('AccountAggregate.Order') is not null drop table AccountAggregate.[Order]
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
