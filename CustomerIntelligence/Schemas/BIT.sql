-- create schema
if not exists (select * from sys.schemas where name = 'BIT') exec('create schema BIT')

-- drop tables
if object_id('BIT.ProjectCategoryStatistics') is not null drop table BIT.ProjectCategoryStatistics
if object_id('BIT.FirmCategoryStatistics') is not null drop table BIT.FirmCategoryStatistics
if object_id('BIT.FirmForecast') is not null drop table BIT.FirmForecast
if object_id('BIT.FirmCategoryForecast') is not null drop table BIT.FirmCategoryForecast
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

-- FirmForecast
create table BIT.FirmForecast(
    ProjectId bigint not null,
    FirmId bigint not null,
    ForecastClick int not null,
    ForecastAmount decimal(19,4) not null,
    constraint PK_FirmForecast primary key (FirmId)
)
go

-- FirmCategoryForecast
create table BIT.FirmCategoryForecast(
    ProjectId bigint not null,
    FirmId bigint not null,
    CategoryId bigint not null,
    ForecastClick int not null,
    ForecastAmount decimal(19,4) not null,
    constraint PK_FirmCategoryForecast primary key (FirmId, CategoryId)
)
go
