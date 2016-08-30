if not exists (select * from sys.schemas where name = 'ConsistencyFacts') exec('create schema ConsistencyFacts')
go

if object_id('ConsistencyFacts.Order') is not null drop table ConsistencyFacts.[Order]
go

create table ConsistencyFacts.[Order](
    Id bigint not null,
    constraint PK_Order primary key (Id)
)
go
