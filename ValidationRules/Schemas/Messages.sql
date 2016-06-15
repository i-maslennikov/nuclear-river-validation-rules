if not exists (select * from sys.schemas where name = 'Messages') exec('create schema Messages')

if object_id('Messages.Version') is not null drop table Messages.Version
if object_id('Messages.ErmState') is not null drop table Messages.ErmState
if object_id('Messages.ValidationResult') is not null drop table Messages.ValidationResult
go

create table Messages.Version(
    Id bigint NOT NULL,

    constraint PK_Version primary key (Id)
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

    OrderId bigint NOT NULL,
    PeriodStart datetime2(2) NOT NULL,
    PeriodEnd datetime2(2) NOT NULL,
    ProjectId bigint NOT NULL,

    ReferenceType int NOT NULL,
    ReferenceId bigint NOT NULL,

    Result int NOT NULL,
)
go
