if not exists (select * from sys.schemas where name = 'ConsistencyAggregates') exec('create schema ConsistencyAggregates')

if object_id('ConsistencyAggregates.Order') is not null drop table ConsistencyAggregates.[Order]
if object_id('ConsistencyAggregates.InvalidFirm') is not null drop table ConsistencyAggregates.InvalidFirm
if object_id('ConsistencyAggregates.InvalidBeginDistributionDate') is not null drop table ConsistencyAggregates.InvalidBeginDistributionDate
if object_id('ConsistencyAggregates.InvalidEndDistributionDate') is not null drop table ConsistencyAggregates.InvalidEndDistributionDate
if object_id('ConsistencyAggregates.LegalPersonProfileBargainExpired') is not null drop table ConsistencyAggregates.LegalPersonProfileBargainExpired
if object_id('ConsistencyAggregates.LegalPersonProfileWarrantyExpired') is not null drop table ConsistencyAggregates.LegalPersonProfileWarrantyExpired
if object_id('ConsistencyAggregates.BargainSignedLaterThanOrder') is not null drop table ConsistencyAggregates.BargainSignedLaterThanOrder
if object_id('ConsistencyAggregates.MissingBargainScan') is not null drop table ConsistencyAggregates.MissingBargainScan
if object_id('ConsistencyAggregates.MissingOrderScan') is not null drop table ConsistencyAggregates.MissingOrderScan
if object_id('ConsistencyAggregates.HasNoAnyLegalPersonProfile') is not null drop table ConsistencyAggregates.HasNoAnyLegalPersonProfile
if object_id('ConsistencyAggregates.HasNoAnyPosition') is not null drop table ConsistencyAggregates.HasNoAnyPosition
if object_id('ConsistencyAggregates.NoReleasesSheduled') is not null drop table ConsistencyAggregates.NoReleasesSheduled
if object_id('ConsistencyAggregates.MissingBills') is not null drop table ConsistencyAggregates.MissingBills
if object_id('ConsistencyAggregates.InvalidBillsTotal') is not null drop table ConsistencyAggregates.InvalidBillsTotal
if object_id('ConsistencyAggregates.InvalidBillsPeriod') is not null drop table ConsistencyAggregates.InvalidBillsPeriod
if object_id('ConsistencyAggregates.MissingRequiredField') is not null drop table ConsistencyAggregates.MissingRequiredField
go

create table ConsistencyAggregates.[Order](
    Id bigint not null,
    ProjectId bigint not null,
    Number nvarchar(max) not null,
    BeginDistribution datetime2(2) not null,
    EndDistributionFact datetime2(2) not null,
    EndDistributionPlan datetime2(2) not null,
)
go

create table ConsistencyAggregates.InvalidFirm(
    OrderId bigint not null,
    FirmId bigint not null,
    [State] int not null,
    Name nvarchar(max) not null,
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
    LegalPersonProfileName nvarchar(max) not null,
)
go

create table ConsistencyAggregates.LegalPersonProfileWarrantyExpired(
    OrderId bigint not null,
    LegalPersonProfileId bigint not null,
    LegalPersonProfileName nvarchar(max) not null,
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

create table ConsistencyAggregates.NoReleasesSheduled(
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
)
go
