if not exists (select * from sys.schemas where name = 'ThemeAggregates') exec('create schema ThemeAggregates')
go

if object_id('ThemeAggregates.Theme') is not null drop table ThemeAggregates.Theme
if object_id('ThemeAggregates.InvalidCategory') is not null drop table ThemeAggregates.InvalidCategory

if object_id('ThemeAggregates.Order') is not null drop table ThemeAggregates.[Order]
if object_id('ThemeAggregates.OrderTheme') is not null drop table ThemeAggregates.OrderTheme

if object_id('ThemeAggregates.Project') is not null drop table ThemeAggregates.Project
if object_id('ThemeAggregates.ProjectTheme') is not null drop table ThemeAggregates.ProjectTheme

if object_id('ThemeAggregates.Category') is not null drop table ThemeAggregates.Category
go

-- Theme aggregate
create table ThemeAggregates.Theme (
    Id bigint not null,

    Name nvarchar(64) not null,
	BeginDistribution datetime2(2) not null,
	EndDistribution datetime2(2) not null,

	IsDefault bit not null,
)
go
create table ThemeAggregates.InvalidCategory (
    ThemeId bigint not null,
    CategoryId bigint not null,
)
go

-- Order aggregate
create table ThemeAggregates.[Order] (
    Id bigint not null,

    Number nvarchar(64) not null,
	BeginDistributionDate datetime2(2) not null,
	EndDistributionDateFact datetime2(2) not null,

	SourceProjectId bigint not null,
	DestProjectId bigint not null,

	IsSelfAds bit not null,
)
go
create table ThemeAggregates.OrderTheme (
    OrderId bigint not null,
    ThemeId bigint not null,
)
go

-- Project aggregate
create table ThemeAggregates.Project (
    Id bigint not null,

    Name nvarchar(64) not null,
)
go
create table ThemeAggregates.ProjectTheme (
	ProjectId bigint not null,
    ThemeId bigint not null,
)
go

-- Category aggregate
create table ThemeAggregates.Category (
    Id bigint not null,

    Name nvarchar(128) not null,
)
go
