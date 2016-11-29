if object_id('AccountFacts.Order') is not null drop table AccountFacts.[Order]
if object_id('AccountFacts.Project') is not null drop table AccountFacts.Project
if object_id('AccountFacts.Account') is not null drop table AccountFacts.Account
if object_id('AccountFacts.Lock') is not null drop table AccountFacts.Lock
if object_id('AccountFacts.ReleaseWithdrawal') is not null drop table AccountFacts.ReleaseWithdrawal
if object_id('AccountFacts.OrderPosition') is not null drop table AccountFacts.OrderPosition
if object_id('AccountFacts.UnlimitedOrder') is not null drop table AccountFacts.UnlimitedOrder
go

if exists (select * from sys.schemas where name = 'AccountFacts') exec('drop schema AccountFacts')
go

if object_id('AdvertisementFacts.AdvertisementTemplate') is not null drop table AdvertisementFacts.AdvertisementTemplate
if object_id('AdvertisementFacts.Position') is not null drop table AdvertisementFacts.Position
if object_id('AdvertisementFacts.PricePosition') is not null drop table AdvertisementFacts.PricePosition
if object_id('AdvertisementFacts.OrderPositionAdvertisement') is not null drop table AdvertisementFacts.OrderPositionAdvertisement
if object_id('AdvertisementFacts.OrderPosition') is not null drop table AdvertisementFacts.OrderPosition
if object_id('AdvertisementFacts.Order') is not null drop table AdvertisementFacts.[Order]
if object_id('AdvertisementFacts.Project') is not null drop table AdvertisementFacts.Project
if object_id('AdvertisementFacts.Advertisement') is not null drop table AdvertisementFacts.Advertisement
if object_id('AdvertisementFacts.Firm') is not null drop table AdvertisementFacts.Firm
if object_id('AdvertisementFacts.FirmAddress') is not null drop table AdvertisementFacts.FirmAddress
if object_id('AdvertisementFacts.FirmAddressWebsite') is not null drop table AdvertisementFacts.FirmAddressWebsite
if object_id('AdvertisementFacts.AdvertisementElement') is not null drop table AdvertisementFacts.AdvertisementElement
if object_id('AdvertisementFacts.AdvertisementElementTemplate') is not null drop table AdvertisementFacts.AdvertisementElementTemplate
go

if exists (select * from sys.schemas where name = 'AdvertisementFacts') exec('drop schema AdvertisementFacts')
go

if object_id('ConsistencyFacts.Bargain') is not null drop table ConsistencyFacts.Bargain
if object_id('ConsistencyFacts.BargainScanFile') is not null drop table ConsistencyFacts.BargainScanFile
if object_id('ConsistencyFacts.Bill') is not null drop table ConsistencyFacts.Bill
if object_id('ConsistencyFacts.BranchOffice') is not null drop table ConsistencyFacts.BranchOffice
if object_id('ConsistencyFacts.BranchOfficeOrganizationUnit') is not null drop table ConsistencyFacts.BranchOfficeOrganizationUnit
if object_id('ConsistencyFacts.Firm') is not null drop table ConsistencyFacts.Firm
if object_id('ConsistencyFacts.Category') is not null drop table ConsistencyFacts.Category
if object_id('ConsistencyFacts.CategoryFirmAddress') is not null drop table ConsistencyFacts.CategoryFirmAddress
if object_id('ConsistencyFacts.Deal') is not null drop table ConsistencyFacts.Deal
if object_id('ConsistencyFacts.FirmAddress') is not null drop table ConsistencyFacts.FirmAddress
if object_id('ConsistencyFacts.LegalPerson') is not null drop table ConsistencyFacts.LegalPerson
if object_id('ConsistencyFacts.LegalPersonProfile') is not null drop table ConsistencyFacts.LegalPersonProfile
if object_id('ConsistencyFacts.[Order]') is not null drop table ConsistencyFacts.[Order]
if object_id('ConsistencyFacts.OrderPosition') is not null drop table ConsistencyFacts.OrderPosition
if object_id('ConsistencyFacts.OrderPositionAdvertisement') is not null drop table ConsistencyFacts.OrderPositionAdvertisement
if object_id('ConsistencyFacts.OrderScanFile') is not null drop table ConsistencyFacts.OrderScanFile
if object_id('ConsistencyFacts.Position') is not null drop table ConsistencyFacts.Position
if object_id('ConsistencyFacts.Project') is not null drop table ConsistencyFacts.Project
if object_id('ConsistencyFacts.ReleaseWithdrawal') is not null drop table ConsistencyFacts.ReleaseWithdrawal
go

if exists (select * from sys.schemas where name = 'ConsistencyFacts') exec('drop schema ConsistencyFacts')
go

if object_id('FirmFacts.Firm') is not null drop table FirmFacts.Firm
if object_id('FirmFacts.FirmAddress') is not null drop table FirmFacts.FirmAddress
if object_id('FirmFacts.FirmAddressCategory') is not null drop table FirmFacts.FirmAddressCategory
if object_id('FirmFacts.[Order]') is not null drop table FirmFacts.[Order]
if object_id('FirmFacts.OrderPosition') is not null drop table FirmFacts.OrderPosition
if object_id('FirmFacts.OrderPositionAdvertisement') is not null drop table FirmFacts.OrderPositionAdvertisement
if object_id('FirmFacts.SpecialPosition') is not null drop table FirmFacts.SpecialPosition
if object_id('FirmFacts.Project') is not null drop table FirmFacts.Project
go

if exists (select * from sys.schemas where name = 'FirmFacts') exec('drop schema FirmFacts')
go

if object_id('PriceFacts.AssociatedPosition') is not null drop table PriceFacts.AssociatedPosition
if object_id('PriceFacts.AssociatedPositionsGroup') is not null drop table PriceFacts.AssociatedPositionsGroup
if object_id('PriceFacts.DeniedPosition') is not null drop table PriceFacts.DeniedPosition
if object_id('PriceFacts.OrderPositionAdvertisement') is not null drop table PriceFacts.OrderPositionAdvertisement
if object_id('PriceFacts.OrderPosition') is not null drop table PriceFacts.OrderPosition
if object_id('PriceFacts.Order') is not null drop table PriceFacts.[Order]
if object_id('PriceFacts.PricePosition') is not null drop table PriceFacts.PricePosition
if object_id('PriceFacts.PricePositionNotActive') is not null drop table PriceFacts.PricePositionNotActive
if object_id('PriceFacts.Price') is not null drop table PriceFacts.Price
if object_id('PriceFacts.Project') is not null drop table PriceFacts.Project
if object_id('PriceFacts.Position') is not null drop table PriceFacts.Position
if object_id('PriceFacts.Category') is not null drop table PriceFacts.Category
if object_id('PriceFacts.RulesetRule') is not null drop table PriceFacts.RulesetRule
if object_id('PriceFacts.Theme') is not null drop table PriceFacts.Theme
go

if exists (select * from sys.schemas where name = 'PriceFacts') exec('drop  schema PriceFacts')
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

if exists (select * from sys.schemas where name = 'ProjectFacts') exec('drop schema ProjectFacts')
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

if exists (select * from sys.schemas where name = 'ThemeFacts') exec('drop schema ThemeFacts')
go

if object_id('UserContext.UserAccount') is not null drop table UserContext.UserAccount
if object_id('UserContext.UserProfile') is not null drop table UserContext.UserProfile
if object_id('UserContext.UserOrder') is not null drop table UserContext.UserOrder
go

if exists (select * from sys.schemas where name = 'UserContext') exec('drop schema UserContext')
go

if exists (select * from sys.schemas where name = 'PriceContext') exec('drop schema PriceContext')
go
if exists (select * from sys.schemas where name = 'PriceAggregate') exec('drop schema PriceAggregate')
go
if exists (select * from sys.schemas where name = 'AccountContext') exec('drop schema AccountContext')
go
if exists (select * from sys.schemas where name = 'AccountAggregate') exec('drop schema AccountAggregate')
go
