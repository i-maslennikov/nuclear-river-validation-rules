if not exists (select * from sys.schemas where name = 'Messages') exec('create schema Messages')

if object_id('Messages.ValidationResult') is not null drop table Messages.ValidationResult
if object_id('Messages.HistoryValidationResult') is not null drop table Messages.HistoryValidationResult

go

create table Messages.ValidationResult(
    OrderId bigint NOT NULL,
    PeriodStart datetime2(2) NOT NULL,
    PeriodEnd datetime2(2) NOT NULL,
    MessageType int NOT NULL,

    ProjectId bigint NOT NULL,
    IsFailed bit NOT NULL,
    MessageParams xml NULL,

    constraint PK_ValidationResult primary key (OrderId, PeriodStart, PeriodEnd, MessageType)
)
go

create table Messages.HistoryValidationResult(
    OrderId bigint NOT NULL,
    PeriodStart datetime2(2) NOT NULL,
    PeriodEnd datetime2(2) NOT NULL,
    MessageType int NOT NULL,

    OrderVersion nvarchar(64) NOT NULL,
    IsFailed bit NOT NULL,
    MessageParams xml NULL,

    constraint PK_HistoryValidationResult primary key (OrderId, PeriodStart, PeriodEnd, MessageType, OrderVersion)
)
go
