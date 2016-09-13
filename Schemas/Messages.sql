if not exists (select * from sys.schemas where name = 'Messages') exec('create schema Messages')

if object_id('Messages.ValidationResultByOrder', 'view') is not null drop view Messages.ValidationResultByOrder
go

if object_id('Messages.Version') is not null drop table Messages.Version
if object_id('Messages.ErmState') is not null drop table Messages.ErmState
if object_id('Messages.ValidationResult') is not null drop table Messages.ValidationResult
go

create table Messages.Version(
    Id bigint NOT NULL,

    constraint PK_Version primary key (Id desc)
)
go

create table Messages.ErmState(
    VersionId bigint NOT NULL,
    Token uniqueidentifier NOT NULL,
)
go

create table Messages.ValidationResult(
    VersionId bigint NOT NULL,

    MessageType int NOT NULL,
    MessageParams xml NOT NULL,

    PeriodStart datetime2(2) NOT NULL,
    PeriodEnd datetime2(2) NOT NULL,
    ProjectId bigint NOT NULL,

    Result int NOT NULL,
)
go

create view Messages.ValidationResultByOrder as 
    select Messages.ValidationResult.*, [Order].value('./@id', 'bigint') as OrderId
    from Messages.ValidationResult
        cross apply MessageParams.nodes('/root/order') as T1([Order])
go