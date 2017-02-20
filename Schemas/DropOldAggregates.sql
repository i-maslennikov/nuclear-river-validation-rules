if object_id('ThemeAggregates.Category') is not null drop table ThemeAggregates.Category

if object_id('ProjectAggregates.Position') is not null drop table ProjectAggregates.Position
if object_id('ProjectAggregates.Category') is not null drop table ProjectAggregates.Category

if object_id('PriceAggregates.Theme') is not null drop table PriceAggregates.Theme
if object_id('PriceAggregates.Project') is not null drop table PriceAggregates.Project
if object_id('PriceAggregates.Category') is not null drop table PriceAggregates.Category

if object_id('AdvertisementAggregates.AdvertisementElementTemplate') is not null drop table AdvertisementAggregates.AdvertisementElementTemplate
if object_id('AdvertisementAggregates.Position') is not null drop table AdvertisementAggregates.Position
if object_id('AdvertisementAggregates.ElementOffsetInDays') is not null drop table AdvertisementAggregates.ElementOffsetInDays
if object_id('AdvertisementAggregates.ElementPeriod') is not null drop table AdvertisementAggregates.ElementPeriod
if object_id('AdvertisementAggregates.LinkedProject') is not null drop table AdvertisementAggregates.LinkedProject
if object_id('AdvertisementAggregates.OrderAdvertisement') is not null drop table AdvertisementAggregates.OrderAdvertisement

if object_id('FirmAggregates.SpecialPosition') is not null drop table FirmAggregates.SpecialPosition

if object_id('ConsistencyAggregates.InvalidCategoryFirmAddress') is not null drop table ConsistencyAggregates.InvalidCategoryFirmAddress

if object_id('ConsistencyAggregates.InvalidFirm') is not null drop table ConsistencyAggregates.InvalidFirm
go
