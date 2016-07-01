if not exists (select * from sys.schemas where name = 'PriceAggregate') exec('create schema PriceAggregate')

if object_id('AccountAggregate.Order') is not null drop table AccountAggregate.[Order]
go

create table PriceContext.[Order](
    Id bigint not null,
    Number nvarchar(max) not null,
    HasAccount bit not null,
    constraint PK_Order primary key (Id)
)
go
