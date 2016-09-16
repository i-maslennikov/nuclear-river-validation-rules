if not exists (select * from sys.schemas where name = 'ConsistencyAggregates') exec('create schema ConsistencyAggregates')

if object_id('ConsistencyAggregates.Order') is not null drop table ConsistencyAggregates.[Order]
if object_id('ConsistencyAggregates.InvalidFirm') is not null drop table ConsistencyAggregates.InvalidFirm
if object_id('ConsistencyAggregates.InvalidFirmAddress') is not null drop table ConsistencyAggregates.InvalidFirmAddress
if object_id('ConsistencyAggregates.InvalidCategory') is not null drop table ConsistencyAggregates.InvalidCategory
if object_id('ConsistencyAggregates.InvalidCategoryFirmAddress') is not null drop table ConsistencyAggregates.InvalidCategoryFirmAddress
if object_id('ConsistencyAggregates.InvalidBeginDistributionDate') is not null drop table ConsistencyAggregates.InvalidBeginDistributionDate
if object_id('ConsistencyAggregates.InvalidEndDistributionDate') is not null drop table ConsistencyAggregates.InvalidEndDistributionDate
if object_id('ConsistencyAggregates.LegalPersonProfileBargainExpired') is not null drop table ConsistencyAggregates.LegalPersonProfileBargainExpired
if object_id('ConsistencyAggregates.LegalPersonProfileWarrantyExpired') is not null drop table ConsistencyAggregates.LegalPersonProfileWarrantyExpired
if object_id('ConsistencyAggregates.BargainSignedLaterThanOrder') is not null drop table ConsistencyAggregates.BargainSignedLaterThanOrder
if object_id('ConsistencyAggregates.MissingBargainScan') is not null drop table ConsistencyAggregates.MissingBargainScan
if object_id('ConsistencyAggregates.MissingOrderScan') is not null drop table ConsistencyAggregates.MissingOrderScan
if object_id('ConsistencyAggregates.HasNoAnyLegalPersonProfile') is not null drop table ConsistencyAggregates.HasNoAnyLegalPersonProfile
if object_id('ConsistencyAggregates.HasNoAnyPosition') is not null drop table ConsistencyAggregates.HasNoAnyPosition
if object_id('ConsistencyAggregates.MissingBills') is not null drop table ConsistencyAggregates.MissingBills
if object_id('ConsistencyAggregates.InvalidBillsTotal') is not null drop table ConsistencyAggregates.InvalidBillsTotal
if object_id('ConsistencyAggregates.InvalidBillsPeriod') is not null drop table ConsistencyAggregates.InvalidBillsPeriod
if object_id('ConsistencyAggregates.MissingRequiredField') is not null drop table ConsistencyAggregates.MissingRequiredField
go

create table ConsistencyAggregates.[Order](
    Id bigint not null,
    ProjectId bigint not null,
    Number nvarchar(64) not null,
    BeginDistribution datetime2(2) not null,
    EndDistributionFact datetime2(2) not null,
    EndDistributionPlan datetime2(2) not null,
)
go

create table ConsistencyAggregates.InvalidFirm(
    OrderId bigint not null,
    FirmId bigint not null,
    FirmName nvarchar(250) not null,
    [State] int not null,
)
go

create table ConsistencyAggregates.InvalidFirmAddress(
    OrderId bigint not null,
    FirmAddressId bigint not null,
    FirmAddressName nvarchar(512) not null,
    OrderPositionId bigint not null,
    OrderPositionName nvarchar(256) not null,
    [State] int not null,
)
go

create table ConsistencyAggregates.InvalidCategory(
    OrderId bigint not null,
    CategoryId bigint not null,
    CategoryName nvarchar(128) not null,
    OrderPositionId bigint not null,
    OrderPositionName nvarchar(256) not null,
    [State] int not null,
    MayNotBelongToFirm bit not null,
)
go

create table ConsistencyAggregates.InvalidCategoryFirmAddress(
    OrderId bigint not null,
    FirmAddressId bigint not null,
    FirmAddressName nvarchar(512) not null,
    CategoryId bigint not null,
    CategoryName nvarchar(128) not null,
    OrderPositionId bigint not null,
    OrderPositionName nvarchar(256) not null,
    [State] int not null,
)
go

create table ConsistencyAggregates.InvalidBeginDistributionDate(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.InvalidEndDistributionDate(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.LegalPersonProfileBargainExpired(
    OrderId bigint not null,
    LegalPersonProfileId bigint not null,
    LegalPersonProfileName nvarchar(256) not null,
)
go

create table ConsistencyAggregates.LegalPersonProfileWarrantyExpired(
    OrderId bigint not null,
    LegalPersonProfileId bigint not null,
    LegalPersonProfileName nvarchar(256) not null,
)
go

create table ConsistencyAggregates.BargainSignedLaterThanOrder(
    OrderId bigint not null,
    BargainId bigint not null,
)
go

create table ConsistencyAggregates.MissingBargainScan(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.MissingOrderScan(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.HasNoAnyLegalPersonProfile(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.HasNoAnyPosition(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.MissingBills(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.InvalidBillsTotal(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.InvalidBillsPeriod(
    OrderId bigint not null,
)
go

create table ConsistencyAggregates.MissingRequiredField(
    OrderId bigint not null,
    LegalPerson bit not null,
    LegalPersonProfile bit not null,
    BranchOfficeOrganizationUnit bit not null,
    Inspector bit not null,
    Currency bit not null,
    ReleaseCountPlan bit not null,
)
go
