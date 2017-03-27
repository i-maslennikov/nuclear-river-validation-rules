if not exists (select * from sys.schemas where name = 'WebApp') exec('create schema WebApp')
go

if object_id('WebApp.Lock') is null
    create table WebApp.Lock(
        Id int not null,
        Expires datetime2(2) not null,
        InUse bit not null,
        IsNew bit not null,

        constraint [PK_Lock] primary key (Id)
    )
go