if object_id('ThemeAggregates.Category') is not null drop table ThemeAggregates.Category

if object_id('ProjectAggregates.Position') is not null drop table ProjectAggregates.Position
if object_id('ProjectAggregates.Category') is not null drop table ProjectAggregates.Category

if object_id('PriceAggregates.Theme') is not null drop table PriceAggregates.Theme
if object_id('PriceAggregates.Project') is not null drop table PriceAggregates.Project
if object_id('PriceAggregates.Category') is not null drop table PriceAggregates.Category

if object_id('AdvertisementAggregates.AdvertisementElementTemplate') is not null drop table AdvertisementAggregates.AdvertisementElementTemplate
if object_id('AdvertisementAggregates.Position') is not null drop table AdvertisementAggregates.Position

go
