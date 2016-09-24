if not exists (select * from sys.schemas where name = 'FirmFacts') exec('create schema FirmFacts')
go

if object_id('FirmFacts.Firm') is not null drop table FirmFacts.Firm
if object_id('FirmFacts.FirmAddress') is not null drop table FirmFacts.FirmAddress
if object_id('FirmFacts.FirmAddressCategory') is not null drop table FirmFacts.FirmAddressCategory
if object_id('FirmFacts.[Order]') is not null drop table FirmFacts.[Order]
if object_id('FirmFacts.OrderPosition') is not null drop table FirmFacts.OrderPosition
if object_id('FirmFacts.OrderPositionAdvertisement') is not null drop table FirmFacts.OrderPositionAdvertisement
if object_id('FirmFacts.Position') is not null drop table FirmFacts.Position
if object_id('FirmFacts.Project') is not null drop table FirmFacts.Project
go

create table FirmFacts.Firm(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    Name nvarchar(250) not null,
    constraint PK_Firm primary key (Id)
)
go

create table FirmFacts.FirmAddress(
    Id bigint not null,
    FirmId bigint not null,
    constraint PK_FirmAddress primary key (Id)
)
go

create table FirmFacts.FirmAddressCategory(
    Id bigint not null,
    FirmAddressId bigint not null,
    CategoryId bigint not null,
    constraint PK_FirmAddressCategory primary key (Id)
)
go

create table FirmFacts.[Order](
    Id bigint not null,
    FirmId bigint not null,
    DestOrganizationUnitId bigint not null,
    Number nvarchar(64) not null,
    BeginDistribution datetime2(2) not null,
    EndDistributionFact datetime2(2) not null,
    WorkflowStep int not null,
    constraint PK_Order primary key (Id)
)
go

create table FirmFacts.OrderPosition(
    Id bigint not null,
    OrderId bigint not null,
    constraint PK_OrderPosition primary key (Id)
)
go

create table FirmFacts.OrderPositionAdvertisement(
    Id bigint not null,
    OrderPositionId bigint not null,
    PositionId bigint not null,
    CategoryId bigint null,
    constraint PK_OrderPositionAdvertisement primary key (Id)
)
go

create table FirmFacts.Position(
    Id bigint not null,
    CategoryCode bigint not null,
    [Platform] int null,
    constraint PK_Position primary key (Id)
)
go

create table FirmFacts.Project(
    Id bigint not null,
    OrganizationUnitId bigint not null,
    constraint PK_Project primary key (Id)
)
go

CREATE NONCLUSTERED INDEX IX_OrderPositionAdvertisement_CategoryId
ON [FirmFacts].[OrderPositionAdvertisement] ([CategoryId])
INCLUDE ([OrderPositionId])
GO

CREATE NONCLUSTERED INDEX IX_OrderPositionAdvertisement_OrderPositionId
ON [FirmFacts].[OrderPositionAdvertisement] ([OrderPositionId])
INCLUDE ([PositionId])
GO

CREATE NONCLUSTERED INDEX IX_FirmAddressCategory_CategoryId
ON [FirmFacts].[FirmAddressCategory] ([CategoryId])
INCLUDE ([FirmAddressId])
GO
