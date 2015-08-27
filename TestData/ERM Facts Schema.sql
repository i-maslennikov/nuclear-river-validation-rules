-- create schema
if not exists (select * from sys.schemas where name = 'ERM')
	exec('create schema ERM')
go

-- drop tables
if object_id('ERM.Account') is not null drop table ERM.Account;
if object_id('ERM.Activity') is not null drop table ERM.Activity;
if object_id('ERM.BranchOfficeOrganizationUnit') is not null drop table ERM.BranchOfficeOrganizationUnit;
if object_id('ERM.Category') is not null drop table ERM.Category;
if object_id('ERM.CategoryGroup') is not null drop table ERM.CategoryGroup;
if object_id('ERM.CategoryFirmAddress') is not null drop table ERM.CategoryFirmAddress;
if object_id('ERM.CategoryOrganizationUnit') is not null drop table ERM.CategoryOrganizationUnit;
if object_id('ERM.Client') is not null drop table ERM.Client;
if object_id('ERM.Contact') is not null drop table ERM.Contact;
if object_id('ERM.Firm') is not null drop table ERM.Firm;
if object_id('ERM.FirmAddress') is not null drop table ERM.FirmAddress;
if object_id('ERM.FirmContact') is not null drop table ERM.FirmContact;
if object_id('ERM.LegalPerson') is not null drop table ERM.LegalPerson;
if object_id('ERM.Order') is not null drop table ERM.[Order];
if object_id('ERM.Project') is not null drop table ERM.Project;
if object_id('ERM.Territory') is not null drop table ERM.Territory;
go


-- Account
create table ERM.Account(
	Id bigint not null
    , Balance decimal(19,4) not null
    , BranchOfficeOrganizationUnitId bigint not null
    , LegalPersonId bigint not null
    , constraint PK_Accounts primary key (Id)
)
go

-- Activity
create table ERM.Activity(
	Id bigint not null
    , ModifiedOn datetimeoffset(2) not null
	, FirmId bigint null
	, ClientId bigint null
    , constraint PK_Activities primary key (Id)
)
go

-- BranchOfficeOrganizationUnit
create table ERM.BranchOfficeOrganizationUnit(
	Id bigint not null
    , OrganizationUnitId bigint not null
    , constraint PK_BranchOfficeOrganizationUnits primary key (Id)
)
go

-- Category
create table ERM.Category(
	Id bigint not null
    , Name nvarchar(256) not null
    , [Level] int not null
    , ParentId bigint null
    , constraint PK_Categories primary key (Id)
)
go

-- CategoryGroup
create table ERM.CategoryGroup(
	Id bigint not null
    , Name nvarchar(256) not null
    , Rate float not null
    , constraint PK_CategoryGroups primary key (Id)
)
go

-- CategoryFirmAddress
create table ERM.CategoryFirmAddress(
	Id bigint not null
    , CategoryId bigint not null
    , FirmAddressId bigint not null
    , constraint PK_CategoryFirmAddresses primary key (Id)
)
go
create nonclustered index IX_CategoryFirmAddress_FirmAddressId
on ERM.CategoryFirmAddress (FirmAddressId)
include (CategoryId)
go

-- CategoryOrganizationUnit
create table ERM.CategoryOrganizationUnit(
	Id bigint not null
	, CategoryId bigint not null
    , CategoryGroupId bigint not null
    , OrganizationUnitId bigint not null
    , constraint PK_CategoryOrganizationUnits primary key (Id)
)
go
create nonclustered index IX_CategoryOrganizationUnit_CategoryId_OrganizationUnitId
on ERM.CategoryOrganizationUnit (CategoryId, OrganizationUnitId)
include (CategoryGroupId)
go

-- Client
create table ERM.Client(
	Id bigint not null
    , Name nvarchar(256) not null
    , LastDisqualifiedOn datetimeoffset(2) null
    , HasPhone bit not null constraint DF_Clients_HasPhone default 0
    , HasWebsite bit not null constraint DF_Clients_HasWebsite default 0
    , constraint PK_Clients primary key (Id)
)
go

-- Contact
create table ERM.Contact(
	Id bigint not null
	, [Role] int not null
	, IsFired bit not null constraint DF_Contacts_IsFired default 0
    , HasPhone bit not null constraint DF_Contacts_HasPhone default 0
    , HasWebsite bit not null constraint DF_Contacts_HasWebsite default 0
    , ClientId bigint not null
    , constraint PK_Contacts primary key (Id)
)
go
create nonclustered index IX_Contact_HasPhone_HasWebsite
on ERM.Contact (HasPhone, HasWebsite)
go

-- Firm
create table ERM.Firm(
	Id bigint not null
    , Name nvarchar(256) not null
    , CreatedOn datetimeoffset(2) not null
    , LastDisqualifiedOn datetimeoffset(2) null
    , ClientId bigint null
    , OrganizationUnitId bigint not null
    , OwnerId bigint not null
    , TerritoryId bigint not null
    , constraint PK_Firms primary key (Id)
)
go
create nonclustered index IX_Firm_ClientId_OrganizationUnitId
on ERM.Firm (ClientId, OrganizationUnitId)
include (Id)
go

-- FirmAddress
create table ERM.FirmAddress(
	Id bigint not null
    , FirmId bigint not null
    , constraint PK_FirmAddresses primary key (Id)
)
go
create nonclustered index IX_FirmAddress_FirmId
on ERM.FirmAddress (FirmId)
include (Id)
go

-- FirmContact
create table ERM.FirmContact(
	Id bigint not null
    , HasPhone bit not null constraint DF_FirmContacts_HasPhone default 0
    , HasWebsite bit not null constraint DF_FirmContacts_HasWebsite default 0
    , FirmAddressId bigint not null
    , constraint PK_FirmContacts primary key (Id)
)
go
create nonclustered index IX_FirmContact_HasPhone_FirmAddressId
on ERM.FirmContact (HasPhone, FirmAddressId)
go
create nonclustered index IX_FirmContact_HasWebsite_FirmAddressId
on ERM.FirmContact (HasWebsite,FirmAddressId)
go

-- LegalPerson
create table ERM.LegalPerson(
	Id bigint not null
    , ClientId bigint not null
    , constraint PK_LegalPersons primary key (Id)
)
go

-- Order
create table ERM.[Order](
	Id bigint not null
    , EndDistributionDateFact datetimeoffset(2) not null
    , FirmId bigint null
    , constraint PK_Orders primary key (Id)
)
go

-- Project
create table ERM.Project(
	Id bigint not null
    , Name nvarchar(256) not null
    , OrganizationUnitId bigint not null
    , constraint PK_Projects primary key (Id)
)
go

-- Territory
create table ERM.Territory(
	Id bigint not null
    , Name nvarchar(256) not null
    , OrganizationUnitId bigint not null
    , constraint PK_Territories primary key (Id)
)
go