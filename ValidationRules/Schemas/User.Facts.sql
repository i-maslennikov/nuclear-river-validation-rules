if not exists (select * from sys.schemas where name = 'UserContext') exec('create schema UserContext')
go

if object_id('UserContext.UserAccount') is not null drop table UserContext.UserAccount
if object_id('UserContext.AccountOrder') is not null drop table UserContext.AccountOrder
go

create table UserContext.UserAccount(
    Id bigint not null,
    Name nvarchar(max) not null,
    constraint PK_Account primary key (Id)
)
go

create table UserContext.AccountOrder(
    UserAccountId bigint not null,
    OrderId bigint not null,
    constraint PK_AccountOrder primary key (OrderId)
)
go
