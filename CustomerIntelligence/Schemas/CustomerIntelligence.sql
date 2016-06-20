-- create schema
if not exists (select * from sys.schemas where name = 'CustomerIntelligence') exec('create schema CustomerIntelligence')

-- drop tables
if object_id('CustomerIntelligence.CategoryGroup') is not null drop table CustomerIntelligence.CategoryGroup
if object_id('CustomerIntelligence.Project') is not null drop table CustomerIntelligence.Project
if object_id('CustomerIntelligence.ProjectCategory') is not null drop table CustomerIntelligence.ProjectCategory
if object_id('CustomerIntelligence.Territory') is not null drop table CustomerIntelligence.Territory
if object_id('CustomerIntelligence.Firm') is not null drop table CustomerIntelligence.Firm
if object_id('CustomerIntelligence.FirmBalance') is not null drop table CustomerIntelligence.FirmBalance
if object_id('CustomerIntelligence.FirmLead') is not null drop table CustomerIntelligence.FirmLead
if object_id('CustomerIntelligence.FirmForecast') is not null drop table CustomerIntelligence.FirmForecast
if object_id('CustomerIntelligence.FirmActivity') is not null drop table CustomerIntelligence.FirmActivity
if object_id('CustomerIntelligence.Client') is not null drop table CustomerIntelligence.Client
if object_id('CustomerIntelligence.Contact') is not null drop table CustomerIntelligence.Contact
if object_id('CustomerIntelligence.ClientContact') is not null drop table CustomerIntelligence.ClientContact
if object_id('CustomerIntelligence.FirmCategory') is not null drop table CustomerIntelligence.FirmCategory -- удалить после релиза
if object_id('BIT.FirmCategory', 'view') is not null drop view BIT.FirmCategory -- удалить после релиза
if object_id('CustomerIntelligence.FirmCategory1') is not null drop table CustomerIntelligence.FirmCategory1
if object_id('CustomerIntelligence.FirmCategory2') is not null drop table CustomerIntelligence.FirmCategory2
if object_id('CustomerIntelligence.FirmCategory3') is not null drop table CustomerIntelligence.FirmCategory3
if object_id('CustomerIntelligence.FirmTerritory') is not null drop table CustomerIntelligence.FirmTerritory
if object_id('CustomerIntelligence.ProjectCategoryStatistics') is not null drop table CustomerIntelligence.ProjectCategoryStatistics
if object_id('CustomerIntelligence.ProjectStatistics') is not null drop table CustomerIntelligence.ProjectStatistics
if object_id('CustomerIntelligence.FirmView', 'view') is not null drop view CustomerIntelligence.FirmView
go


-- CategoryGroup
create table CustomerIntelligence.CategoryGroup(
	Id bigint not null
    , Name nvarchar(256) not null
    , Rate float not null
    , constraint PK_CategoryGroups primary key (Id)
)
go

-- Project
create table CustomerIntelligence.Project(
	Id bigint not null
    , Name nvarchar(256) not null
    , constraint PK_Projects primary key (Id)
)
go

-- ProjectCategory
create table CustomerIntelligence.ProjectCategory(
	ProjectId bigint not null
    , CategoryId bigint not null
    , Name nvarchar(256) not null
    , [Level] int not null
    , ParentId bigint null
	, SalesModel int not null
    , constraint PK_ProjectCategories primary key (ProjectId, CategoryId)
)
go

-- Territory
create table CustomerIntelligence.Territory(
	Id bigint not null
    , Name nvarchar(256) not null
    , ProjectId bigint not null
    , constraint PK_Territories primary key (Id)
)
go

-- Firm
create table CustomerIntelligence.Firm(
	Id bigint not null
    , Name nvarchar(256) not null
    , CreatedOn datetimeoffset(2) not null
    , LastDisqualifiedOn datetimeoffset(2) null
	, LastDistributedOn datetimeoffset(2) null
    , HasPhone bit not null constraint DF_Firms_HasPhone default 0
    , HasWebsite bit not null constraint DF_Firms_HasWebsite default 0
    , AddressCount int not null constraint DF_Firms_AddressCount default 0
    , CategoryGroupId bigint not null
    , ClientId bigint null
    , ProjectId bigint not null
    , OwnerId bigint not null
    , constraint PK_Firms primary key (Id)
)
go

-- FirmActivity
create table CustomerIntelligence.FirmActivity(
	FirmId bigint not null
    , LastActivityOn datetimeoffset(2) null
    , constraint PK_FirmActivities primary key (FirmId)
)
go

-- FirmBalance
create table CustomerIntelligence.FirmBalance(
    FirmId bigint not null
    , AccountId bigint not null
	, ProjectId bigint not null
    , Balance decimal(19,4) not null
    , constraint PK_FirmBalances primary key (FirmId, AccountId)
)
go

-- FirmCategory1
create table CustomerIntelligence.FirmCategory1(
	FirmId bigint not null
	, CategoryId bigint not null
    , constraint PK_FirmCategory1 primary key (FirmId, CategoryId)
)
go

-- FirmCategory2
create table CustomerIntelligence.FirmCategory2(
	FirmId bigint not null
	, CategoryId bigint not null
    , constraint PK_FirmCategory2 primary key (FirmId, CategoryId)
)
go

-- ProjectStatistics
create table CustomerIntelligence.ProjectStatistics(
    Id bigint not null
    , constraint PK_ProjectStatistics primary key (Id)
)
go

-- FirmForecast
create table CustomerIntelligence.ProjectCategoryStatistics(
    ProjectId bigint not null
    , CategoryId bigint not null
    , constraint PK_ProjectCategoryStatistics primary key (ProjectId, CategoryId)
)
go

-- FirmCategory3
create table CustomerIntelligence.FirmCategory3(
    ProjectId bigint not null
    , FirmId bigint not null
    , CategoryId bigint not null
    , Name nvarchar(256) not null
    , Hits int not null
    , Shows int not null
    , FirmCount int not null
    , AdvertisersShare float not null
    , ForecastClick int null
    , ForecastAmount decimal(19,4) null
    , constraint PK_FirmCategory3 primary key (FirmId, CategoryId)
)
go

-- FirmForecast
create table CustomerIntelligence.FirmForecast(
    ProjectId bigint not null
    , FirmId bigint not null
    , ForecastClick int not null
    , ForecastAmount decimal(19,4) not null
    , constraint PK_FirmForecast primary key (FirmId)
)
go

-- FirmLead
create table CustomerIntelligence.FirmLead(
	FirmId bigint not null
    , LeadId bigint not null
    , IsInQueue bit not null
    , Type int not null
    , constraint PK_FirmLeads primary key (FirmId, LeadId)
)
go

-- FirmTerritory
create table CustomerIntelligence.FirmTerritory(
	FirmId bigint not null
    , FirmAddressId bigint not null
	, TerritoryId bigint null
    , constraint PK_FirmTerritory primary key (FirmId, FirmAddressId)
)
go

-- Client
create table CustomerIntelligence.Client(
	Id bigint not null
    , Name nvarchar(256) not null
    , CategoryGroupId bigint not null
    , constraint PK_Clients primary key (Id)
)
go

-- ClientContact
create table CustomerIntelligence.ClientContact(
    ClientId bigint not null
    , ContactId bigint not null
    , [Role] int not null
    , constraint PK_ClientContact primary key (ClientId, ContactId)
)
go


-- FirmView
create view CustomerIntelligence.FirmView
as
select Firm.*, FirmActivity.LastActivityOn, FirmForecast.ForecastClick, FirmForecast.ForecastAmount
from CustomerIntelligence.Firm
	inner join CustomerIntelligence.FirmActivity on FirmActivity.FirmId = Firm.Id
	left join CustomerIntelligence.FirmForecast on FirmForecast.FirmId = Firm.Id
go


-- Идексы для клиента
create nonclustered index IX_Quering_1
on CustomerIntelligence.Firm (ProjectId,OwnerId,CreatedOn)
include (Id)
go
create nonclustered index IX_Quering_2
on CustomerIntelligence.Firm (ProjectId,OwnerId,LastDistributedOn)
include (Id)
go
create nonclustered index IX_Quering_3
on CustomerIntelligence.Firm (ProjectId,OwnerId,LastDisqualifiedOn)
include (Id)
go
create nonclustered index IX_Quering_4
on CustomerIntelligence.Firm (LastDistributedOn,ProjectId,OwnerId)
include (Id,Name,LastDisqualifiedOn,ClientId)
go
create nonclustered index IX_Quering_5
on CustomerIntelligence.Firm (ProjectId,OwnerId)
include (Id,Name,LastDisqualifiedOn,ClientId)
go
create nonclustered index IX_Quering_6
on CustomerIntelligence.Firm (HasWebsite,ProjectId,OwnerId)
include (Id)
go
create nonclustered index IX_Quering_7
on CustomerIntelligence.Firm (HasWebsite,ProjectId,OwnerId)
include (Id,Name,LastDisqualifiedOn,ClientId)
go
create nonclustered index IX_Quering_8
on CustomerIntelligence.Firm (ProjectId,OwnerId,AddressCount)
include (Id)
go
create nonclustered index IX_Quering_9
on CustomerIntelligence.Firm (ProjectId,OwnerId,AddressCount)
include (Id,Name,LastDisqualifiedOn,ClientId)
go
create nonclustered index IX_Quering_10
on CustomerIntelligence.FirmBalance (ProjectId,Balance)
include (FirmId)
go
create nonclustered index IX_Quering_11
on CustomerIntelligence.FirmActivity(LastActivityOn)
include (FirmId)
go
create nonclustered index IX_Quering_12
on CustomerIntelligence.ClientContact (Role)
include (ClientId)
go
