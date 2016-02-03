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
			'SourceSubscription' = '9F2C5A2A-924C-485A-9790-9066631DB307'
			'DestTopic' = 'topic.performedoperations'
			'DestSubscription' = '9F2C5A2A-924C-485A-9790-9066631DB307'
			'Transform' = 'None'
		}
		'ERMProduction' = @{
			'SourceTopic' = 'topic.performedoperations.export'
			'SourceSubscription' = $Context.EnvironmentName.ToLowerInvariant()
			'DestTopic' = "topic.performedoperations.production.$($Context.Country).import".ToLowerInvariant()
			'DestSubscription' = '9F2C5A2A-924C-485A-9790-9066631DB307'
			'Transform' = 'None'
		}
	}
}

Export-ModuleMember -Function Get-UseCaseRouteMetadata