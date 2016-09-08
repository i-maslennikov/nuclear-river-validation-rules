if not exists (select * from sys.schemas where name = 'ConsistencyFacts') exec('create schema ConsistencyFacts')
go

if object_id('ConsistencyFacts.Bargain') is not null drop table ConsistencyFacts.Bargain
if object_id('ConsistencyFacts.BargainScanFile') is not null drop table ConsistencyFacts.BargainScanFile
if object_id('ConsistencyFacts.Bill') is not null drop table ConsistencyFacts.Bill
if object_id('ConsistencyFacts.Firm') is not null drop table ConsistencyFacts.Firm
if object_id('ConsistencyFacts.Category') is not null drop table ConsistencyFacts.Category
if object_id('ConsistencyFacts.CategoryFirmAddress') is not null drop table ConsistencyFacts.CategoryFirmAddress
if object_id('ConsistencyFacts.FirmAddress') is not null drop table ConsistencyFacts.FirmAddress
if object_id('ConsistencyFacts.LegalPersonProfile') is not null drop table ConsistencyFacts.LegalPersonProfile
if object_id('ConsistencyFacts.[Order]') is not null drop table ConsistencyFacts.[Order]
if object_id('ConsistencyFacts.OrderPosition') is not null drop table ConsistencyFacts.OrderPosition
if object_id('ConsistencyFacts.OrderPositionAdvertisement') is not null drop table ConsistencyFacts.OrderPositionAdvertisement
if object_id('ConsistencyFacts.OrderScanFile') is not null drop table ConsistencyFacts.OrderScanFile
if object_id('ConsistencyFacts.Position') is not null drop table ConsistencyFacts.Position
if object_id('ConsistencyFacts.Project') is not null drop table ConsistencyFacts.Project
go

create table ConsistencyFacts.Bargain (
    Id bigint not null,
    SignupDate datetime2(2) not null,
)
go

create table ConsistencyFacts.BargainScanFile (
    Id bigint not null,
    BargainId bigint not null,
)
go

create table ConsistencyFacts.Bill (
    Id bigint not null,
    OrderId bigint not null,
    PayablePlan decimal(19,4) not null,
    [Begin] datetime2(2) not null,
    [End] datetime2(2) not null,
)
go

create table ConsistencyFacts.Category (
    Id bigint not null,
    Name nvarchar(max) not null,
    IsActiveNotDeleted bit not null,
)
go

create table ConsistencyFacts.Firm (
    Id bigint not null,
    IsClosedForAscertainment bit not null,
    IsActive bit not null,
    IsDeleted bit not null,
    Name nvarchar(max) not null,
)
go

create table ConsistencyFacts.FirmAddress (
    Id bigint not null,
    FirmId bigint not null,
    Name nvarchar(max) not null,
    IsClosedForAscertainment bit not null,
    IsActive bit not null,
    IsDeleted bit not null,
)
go

create table ConsistencyFacts.CategoryFirmAddress (
    Id bigint not null,
    FirmAddressId bigint not null,
    CategoryId bigint not null,
)
go

create table ConsistencyFacts.LegalPersonProfile (
    Id bigint not null,
    LegalPersonId bigint not null,
    BargainEndDate datetime2(2) null,
    WarrantyEndDate datetime2(2) null,
    Name nvarchar(max) not null,
)
go

create table ConsistencyFacts.[Order](
    Id bigint not null,
    FirmId bigint not null,
    DestOrganizationUnitId bigint not null,
    LegalPersonId bigint null,
    LegalPersonProfileId bigint null,
    BranchOfficeOrganizationUnitId bigint null,
    InspectorId bigint null,
    CurrencyId bigint null,
    BargainId bigint null,
    
    SignupDate datetime2(2) not null,
    BeginDistribution datetime2(2) not null,
    EndDistributionFact datetime2(2) not null,
    EndDistributionPlan datetime2(2) not null,
    ReleaseCountPlan int not null,
    Number nvarchar(max) not null,
)
go

create table ConsistencyFacts.OrderPosition (
    Id bigint not null,
    OrderId bigint not null,
)
go

create table ConsistencyFacts.OrderPositionAdvertisement (
    Id bigint not null,
    OrderPositionId bigint not null,
    FirmAddressId bigint null,
    CategoryId bigint null,
    PositionId bigint null,
)
go

create table ConsistencyFacts.OrderScanFile (
    Id bigint not null,
    OrderId bigint not null,
)
go

create table ConsistencyFacts.Position (
    Id bigint not null,
    BindingObjectType int not null,
)
go

create table ConsistencyFacts.Project (
    Id bigint not null,
    OrganizationUnitId bigint not null,
)
go
