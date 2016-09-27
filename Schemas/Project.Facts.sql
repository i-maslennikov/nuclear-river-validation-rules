if not exists (select * from sys.schemas where name = 'ProjectFacts') exec('create schema ProjectFacts')
go

if object_id('ProjectFacts.Firm') is not null drop table ProjectFacts.Firm
if object_id('ProjectFacts.[Order]') is not null drop table ProjectFacts.[Order]
go

create table ProjectFacts.Firm(
    Id bigint not null,
    constraint PK_Firm primary key (Id)
)
go

create table FirmFacts.[Order](
    Id bigint not null,
    constraint PK_Order primary key (Id)
)
go
