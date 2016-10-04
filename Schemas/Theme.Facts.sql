if not exists (select * from sys.schemas where name = 'ThemeFacts') exec('create schema ThemeFacts')
go

if object_id('ThemeFacts.Theme') is not null drop table ThemeFacts.Theme
if object_id('ThemeFacts.ThemeCategory') is not null drop table ThemeFacts.ThemeCategory
if object_id('ThemeFacts.ThemeOrganizationUnit') is not null drop table ThemeFacts.ThemeOrganizationUnit
if object_id('ThemeFacts.Category') is not null drop table ThemeFacts.Category
if object_id('ThemeFacts.Order') is not null drop table ThemeFacts.[Order]
if object_id('ThemeFacts.Project') is not null drop table ThemeFacts.Project
if object_id('ThemeFacts.OrderPosition') is not null drop table ThemeFacts.OrderPosition
if object_id('ThemeFacts.OrderPositionAdvertisement') is not null drop table ThemeFacts.OrderPositionAdvertisement

go

create table ThemeFacts.Theme (
    Id bigint not null,

	Name nvarchar(64) not null,
	BeginDistribution datetime2 not null,
	EndDistribution datetime2 not null,
	IsDefault bit not null,

    constraint PK_Theme primary key (Id)
)
go

create table ThemeFacts.ThemeCategory (
    Id bigint not null,

	ThemeId bigint not null,
	CategoryId bigint not null,

    constraint PK_ThemeCategory primary key (Id)
)
go

create table ThemeFacts.ThemeOrganizationUnit (
    Id bigint not null,

	ThemeId bigint not null,
	OrganizationUnitId bigint not null,

    constraint PK_ThemeOrganizationUnit primary key (Id)
)
go

create table ThemeFacts.Category (
    Id bigint not null,

	Name nvarchar(128) not null,
	IsInvalid bit not null,

    constraint PK_Category primary key (Id)
)
go

create table ThemeFacts.[Order] (
    Id bigint not null,

	Number nvarchar(64) not null,

	BeginDistributionDate datetime2(2) not null,
	EndDistributionDateFact datetime2(2) not null,
	SourceOrganizationUnitId bigint not null,
	DestOrganizationUnitId bigint not null,

	IsSelfAds bit not null,

    constraint PK_Order primary key (Id)
)
go

create table ThemeFacts.Project (
    Id bigint not null,
    OrganizationUnitId bigint not null,
	Name nvarchar(64) not null,

    constraint PK_Project primary key (Id)
)
go

create table ThemeFacts.OrderPosition (
    Id bigint not null,
    OrderId bigint not null,

    constraint PK_OrderPosition primary key (Id)
)
go

create table ThemeFacts.OrderPositionAdvertisement (
    Id bigint not null,
    OrderPositionId bigint not null,
	ThemeId bigint not null,

    constraint PK_OrderPositionAdvertisement primary key (Id)
)
go
