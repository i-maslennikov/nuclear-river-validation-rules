-- create schema
if not exists (select * from sys.schemas where name = 'Transport') exec('create schema Transport')

-- drop tables
if object_id('Transport.PerformedOperationFinalProcessing') is not null drop table Transport.PerformedOperationFinalProcessing
go

-- PerformedOperationFinalProcessings
create table Transport.PerformedOperationFinalProcessing(
	Id bigint identity(1,1) not null,
	MessageFlowId uniqueidentifier not null,
	EntityTypeId int not null,
	EntityId bigint not null,
	Context xml null,
	AttemptCount int not null,
	OperationId uniqueidentifier not null,
	CreatedOn datetime2(2) not null,
    constraint PK_PerformedOperationFinalProcessings primary key (Id)
)
go
