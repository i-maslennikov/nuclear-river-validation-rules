if not exists (select * from sys.schemas where name = 'ProjectAggregates') exec('create schema ProjectAggregates')

if object_id('ProjectAggregates.Order') is not null drop table ProjectAggregates.[Order]
if object_id('ProjectAggregates.Firm') is not null drop table ProjectAggregates.Firm
go

create table ProjectAggregates.[Order](
    Id bigint not null,
    constraint PK_Order primary key (Id)
)
go

create table ProjectAggregates.Firm(
    Id bigint not null,
    constraint PK_Firm primary key (Id)
)
go
