if not exists (select * from sys.schemas where name = 'ProjectFacts') exec('create schema ProjectFacts')
go

if object_id('ProjectFacts.Category') is not null drop table ProjectFacts.Category
if object_id('ProjectFacts.CategoryOrganizationUnit') is not null drop table ProjectFacts.CategoryOrganizationUnit
if object_id('ProjectFacts.CostPerClickCategoryRestriction') is not null drop table ProjectFacts.CostPerClickCategoryRestriction
if object_id('ProjectFacts.SalesModelCategoryRestriction') is not null drop table ProjectFacts.SalesModelCategoryRestriction
if object_id('ProjectFacts.FirmAddress') is not null drop table ProjectFacts.FirmAddress
if object_id('ProjectFacts.[Order]') is not null drop table ProjectFacts.[Order]
if object_id('ProjectFacts.OrderPosition') is not null drop table ProjectFacts.OrderPosition
if object_id('ProjectFacts.OrderPositionAdvertisement') is not null drop table ProjectFacts.OrderPositionAdvertisement
if object_id('ProjectFacts.OrderPositionCostPerClick') is not null drop table ProjectFacts.OrderPositionCostPerClick
if object_id('ProjectFacts.Position') is not null drop table ProjectFacts.Position
if object_id('ProjectFacts.PricePosition') is not null drop table ProjectFacts.PricePosition
if object_id('ProjectFacts.Project') is not null drop table ProjectFacts.Project
if object_id('ProjectFacts.ReleaseInfo') is not null drop table ProjectFacts.ReleaseInfo
go

create table ProjectFacts.Category(
    Id bigint not null,
    [Level] int not null,
    Name nvarchar(max) not null,
)
go

create table ProjectFacts.CategoryOrganizationUnit(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    CategoryId bigint not null,
)
go

create table ProjectFacts.CostPerClickCategoryRestriction(
    ProjectId bigint not null,
    CategoryId bigint not null,
    [Begin] datetime2(2) not null,
    MinCostPerClick decimal(19,4) not null,
)
go

create table ProjectFacts.SalesModelCategoryRestriction(
    ProjectId bigint not null,
    CategoryId bigint not null,
    [Begin] datetime2(2) not null,
    SalesModel int not null,
)
go

create table ProjectFacts.FirmAddress(
    Id bigint not null,
    Name nvarchar(max) not null,
    IsLocatedOnTheMap bit not null,
)
go

create table ProjectFacts.[Order](
    Id bigint not null,
    DestOrganizationUnitId bigint not null,
    Number nvarchar(max) not null,
    BeginDistribution datetime2(2) not null,
    EndDistributionPlan datetime2(2) not null,
    WorkflowStep int not null,
)
go

create table ProjectFacts.OrderPosition(
    Id bigint not null,
    OrderId bigint not null,
    PricePositionId bigint not null,
)
go

create table ProjectFacts.OrderPositionAdvertisement(
    Id bigint not null,
    OrderPositionId bigint not null,
    PositionId bigint not null,
    FirmAddressId bigint null,
    CategoryId bigint null,
)
go

create table ProjectFacts.OrderPositionCostPerClick(
    OrderPositionId bigint not null,
    CategoryId bigint not null,
    Amount decimal(19,4) not null,
)
go

create table ProjectFacts.Position(
    Id bigint not null,
    CategoryCode bigint not null,
    SalesModel int not null,
    PositionsGroup int not null,
    Name nvarchar(max) not null,
)
go

create table ProjectFacts.PricePosition(
    Id bigint not null,
    PositionId bigint not null,
)
go

create table ProjectFacts.Project(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    Name nvarchar(max) not null,
)
go

create table ProjectFacts.ReleaseInfo(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    PeriodEndDate datetime2(2) not null,
)
go
