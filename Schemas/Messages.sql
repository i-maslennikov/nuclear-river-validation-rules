if not exists (select * from sys.schemas where name = 'Messages') exec('create schema Messages')

if object_id('Messages.Version') is not null drop table Messages.Version
if object_id('Messages.ErmState') is not null drop table Messages.ErmState
if object_id('Messages.ValidationResult') is not null drop table Messages.ValidationResult
go

create table Messages.Version(
    Id bigint NOT NULL,
    [Date] datetime2(2) NOT NULL,

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
    MessageParams nvarchar(4000) NOT NULL,

    PeriodStart datetime2(2) NOT NULL,
    PeriodEnd datetime2(2) NOT NULL,
    ProjectId bigint NULL,
    OrderId bigint NULL,

    Result int NOT NULL,
    Resolved bit not null,
)
go
CREATE INDEX [IX_ValidationResult_Resolved] ON [Messages].[ValidationResult] ([Resolved])