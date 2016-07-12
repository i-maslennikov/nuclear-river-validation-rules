if not exists (select * from sys.schemas where name = 'UserContext') exec('create schema UserContext')
go

if object_id('UserContext.UserAccount') is not null drop table UserContext.UserAccount
if object_id('UserContext.UserProfile') is not null drop table UserContext.UserProfile
if object_id('UserContext.UserOrder') is not null drop table UserContext.UserOrder
go

create table UserContext.UserAccount(
    Id bigint not null,
    Name nvarchar(max) not null,
    constraint PK_Account primary key (Id)
)
go

create table UserContext.UserOrder(
    UserId bigint not null,
    OrderId bigint not null,
    constraint PK_UserOrder primary key (OrderId)
)
go

create table UserContext.UserProfile(
    Id bigint not null,
    UserId bigint not null,
    TimeZoneId nvarchar(max) not null,
    constraint PK_UserProfile primary key (UserId)
)
go
