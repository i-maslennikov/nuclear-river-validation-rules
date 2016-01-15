-- create schema
if not exists (select * from sys.schemas where name = 'BIT') exec('create schema BIT')

-- drop tables
if object_id('BIT.ProjectCategoryStatistics') is not null drop table BIT.ProjectCategoryStatistics
if object_id('BIT.FirmCategoryStatistics') is not null drop table BIT.FirmCategoryStatistics
go

-- ProjectCategoryStatistics
create table BIT.ProjectCategoryStatistics(
	ProjectId bigint not null,
	CategoryId bigint not null,
    AdvertisersCount bigint not null, 
	constraint PK_ProjectCategoryStatistics primary key (ProjectId, CategoryId)
)
go

-- FirmCategoryStatistics
create table BIT.FirmCategoryStatistics(
	ProjectId bigint not null,
	FirmId bigint not null,
	CategoryId bigint not null,
	Hits int not null,
	Shows int not null,
	constraint PK_FirmCategoryStatistics primary key (FirmId, CategoryId)
)
go