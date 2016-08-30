if not exists (select * from sys.schemas where name = 'ConsistencyAggregates') exec('create schema ConsistencyAggregates')

if object_id('ConsistencyAggregates.Order') is not null drop table ConsistencyAggregates.[Order]
go

create table AccountAggregate.[Order](
    Id bigint not null,
    constraint PK_Order primary key (Id)
)
go
