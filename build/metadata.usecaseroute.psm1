Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

function Get-UseCaseRouteMetadata ($Context) {

	if (!$Context['UseCaseRoute']){
		$Context['UseCaseRoute'] = 'ERM'
	}

	$AllUseCaseRoutes = AllUseCaseRoutes $Context

	$metadata = $AllUseCaseRoutes[$Context.UseCaseRoute]
	if ($metadata -eq $null){
		throw "Can't find route metadata for $($Context.UseCaseRoute)"
	}

	$metadata['RouteName'] = $Context.UseCaseRoute

	return @{
		'UseCaseRoute' = $metadata
		'ProductionUseCaseRoute' = $AllUseCaseRoutes.ERMProduction
	}
}

function AllUseCaseRoutes($Context) {
	return @{
		'ERM' = @{
			'SourceTopic' = 'topic.performedoperations'
			'SourceSubscription' = '6A75B8B4-74A6-4523-9388-84E4DFFD5B06'
			'DestTopic' = 'topic.performedoperations'
			'DestSubscription' = '6A75B8B4-74A6-4523-9388-84E4DFFD5B06'
			'Transform' = 'None'
		}
		'ERMProduction' = @{
			'SourceTopic' = 'topic.performedoperations.export'
			'SourceSubscription' = $Context.EnvironmentName.ToLowerInvariant()
			'DestTopic' = "topic.performedoperations.production.$($Context.Country).import".ToLowerInvariant()
			'DestSubscription' = '6A75B8B4-74A6-4523-9388-84E4DFFD5B06'
			'Transform' = 'None'
		}
	}
}

Export-ModuleMember -Function Get-UseCaseRouteMetadata