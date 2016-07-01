if not exists (select * from sys.schemas where name = 'AccountContext') exec('create schema AccountContext')
go

if object_id('AccountContext.Order') is not null drop table PriceContext.[Order]
if object_id('AccountContext.Account') is not null drop table PriceContext.Account
go

create table PriceContext.[Order](
    Id bigint not null,
    AccountId bigint null,
    Number nvarchar(max) not null,
    constraint PK_Order primary key (Id)
)
go

create table PriceContext.Account(
    Id bigint not null,
    constraint PK_Account primary key (Id)
)
go
