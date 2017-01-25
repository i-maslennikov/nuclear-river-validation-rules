if not exists (select * from sys.schemas where name = 'ProjectAggregates') exec('create schema ProjectAggregates')

if object_id('ProjectAggregates.Category') is not null drop table ProjectAggregates.Category
if object_id('ProjectAggregates.FirmAddress') is not null drop table ProjectAggregates.FirmAddress
if object_id('ProjectAggregates.[Order]') is not null drop table ProjectAggregates.[Order]
if object_id('ProjectAggregates.AddressAdvertisement') is not null drop table ProjectAggregates.AddressAdvertisement
if object_id('ProjectAggregates.CategoryAdvertisement') is not null drop table ProjectAggregates.CategoryAdvertisement
if object_id('ProjectAggregates.CostPerClickAdvertisement') is not null drop table ProjectAggregates.CostPerClickAdvertisement
if object_id('ProjectAggregates.Position') is not null drop table ProjectAggregates.Position
if object_id('ProjectAggregates.Project') is not null drop table ProjectAggregates.Project
if object_id('ProjectAggregates.ProjectCategory') is not null drop table ProjectAggregates.ProjectCategory
if object_id('ProjectAggregates.CostPerClickRestriction') is not null drop table ProjectAggregates.CostPerClickRestriction
if object_id('ProjectAggregates.SalesModelRestriction') is not null drop table ProjectAggregates.SalesModelRestriction
if object_id('ProjectAggregates.NextRelease') is not null drop table ProjectAggregates.NextRelease
go

create table ProjectAggregates.Category(
    Id bigint not null,
    Name nvarchar(max) not null,
	constraint PK_Category primary key (Id)
)
go

create table ProjectAggregates.FirmAddress(
    Id bigint not null,
    Name nvarchar(max) not null,
    IsLocatedOnTheMap bit not null,
    constraint PK_FirmAddress primary key (Id)
)
go

create table ProjectAggregates.[Order](
    Id bigint not null,
    ProjectId bigint not null,
    Number nvarchar(max) not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
    IsDraft bit not null,
    constraint PK_Order primary key (Id)
)
go

create table ProjectAggregates.AddressAdvertisement(
    OrderId bigint not null,
    OrderPositionId bigint not null,
    PositionId bigint not null,
    AddressId bigint not null,
    MustBeLocatedOnTheMap bit not null,
)
go

create table ProjectAggregates.CategoryAdvertisement(
    OrderId bigint not null,
    OrderPositionId bigint not null,
    PositionId bigint not null,
    CategoryId bigint not null,
    SalesModel int not null,
    IsSalesModelRestrictionApplicable bit not null,
)
go

create table ProjectAggregates.CostPerClickAdvertisement(
    OrderId bigint not null,
    OrderPositionId bigint not null,
    PositionId bigint not null,
    CategoryId bigint not null,
    Bid decimal(19,4) not null,
)
go

create table ProjectAggregates.Position(
    Id bigint not null,
    Name nvarchar(max) not null,
    constraint PK_Position primary key (Id)
)
go

create table ProjectAggregates.Project(
    Id bigint not null,
    Name nvarchar(max) not null,
    constraint PK_Project primary key (Id)
)
go

create table ProjectAggregates.ProjectCategory(
    ProjectId bigint not null,
    CategoryId bigint not null,
)
go

create table ProjectAggregates.CostPerClickRestriction(
    ProjectId bigint not null,
    CategoryId bigint not null,
    Minimum decimal(19,4) not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
)
go

create table ProjectAggregates.SalesModelRestriction(
    ProjectId bigint not null,
    CategoryId bigint not null,
    SalesModel int not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
)
go

create table ProjectAggregates.NextRelease(
    ProjectId bigint not null,
    [Date] datetime2(2) not null,
)
go