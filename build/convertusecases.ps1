Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\deploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\servicebus.psm1" -DisableNameChecking

Task Build-ConvertUseCasesService -Precondition { $Metadata['ConvertUseCasesService'] -and $Metadata['UseCaseRoute'] } {

	$сonfig = Get-ConvertUseCasesServiceConfig 'ConvertUseCasesService'

	if ($Metadata.UseCaseRoute.RouteName -match 'ERMProduction'){

		# дополнительная трансформация connection string для production source
		$connectionStringName = 'Source'

		$xmlNode = $сonfig.SelectSingleNode("configuration/connectionStrings/add[@name = '$connectionStringName']")
		if ($xmlNode -eq $null){
			throw "Could not find connection string '$ConnectionStringName'"
		}

		$productionConfig = Get-ConvertUseCasesServiceConfig 'ConvertUseCasesServiceProduction'
		$productionConnectionString = Get-ConnectionString $productionConfig $connectionStringName

		$xmlNode.connectionString = $productionConnectionString
	}

	$tempDir = Copy-ConvertUseCasesServiceToTempDir

	$configFileName = Join-Path $tempDir '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	$сonfig.Save($configFileName)

	Publish-Artifacts $tempDir 'ConvertUseCasesService'
}

function Get-ConvertUseCasesServiceConfig ($metadataKey) {
	$packageInfo = Get-PackageInfo '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases'
	$toolsDir = Join-Path $packageInfo.VersionedDir 'tools'

	$configFileName = Join-Path $toolsDir '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	$transformedConfig = Get-TransformedConfig $configFileName $metadataKey

	return $transformedConfig
}

function Copy-ConvertUseCasesServiceToTempDir  {
	$packageInfo = Get-PackageInfo '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases'
	$toolsDir = Join-Path $packageInfo.VersionedDir 'tools'

	$tempDir = Join-Path $Metadata.Common.Dir.Temp 'ConvertUseCasesService'
	Copy-Item $toolsDir $tempDir -Force -Recurse
	
	return $tempDir
}

Task Create-Topics -Precondition { $Metadata['ConvertUseCasesService'] -and $Metadata['UseCaseRoute'] } {

	$artifacts = Get-Artifacts 'ConvertUseCasesService'
	$configFileName = Join-Path $artifacts '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	[xml]$config = Get-Content $configFileName -Raw

	$useCaseRouteMetadata = $Metadata.UseCaseRoute

	if ($useCaseRouteMetadata.RouteName -notmatch 'ERMProduction'){

		$productionConfig = Get-ConvertUseCasesServiceConfig 'ConvertUseCasesServiceProduction'
		$productionConnectionString = Get-ConnectionString $productionConfig 'Source'

		$productionRouteMetadata = $Metadata.ProductionUseCaseRoute
		Delete-Subscription $productionConnectionString $productionRouteMetadata.SourceTopic $productionRouteMetadata.SourceSubscription
	}

	$sourceConnectionString = Get-ConnectionString $config 'Source'
	Create-Topic $sourceConnectionString $useCaseRouteMetadata.SourceTopic -Properties @{
		'EnableBatchedOperations' = $true
		'SupportOrdering' = $true
	}
	Create-Subscription $sourceConnectionString $useCaseRouteMetadata.SourceTopic $useCaseRouteMetadata.SourceSubscription -Properties @{
		'EnableBatchedOperations' = $true
		'MaxDeliveryCount' = 0x7fffffff
	}

	$destConnectionString = Get-ConnectionString $config 'Dest'
	#Delete-Topic $destConnectionString 'topic.advancedsearch' # временно, потом удалить
	#Delete-Topic $destConnectionString 'topic.performedoperations(.*)import'

	Create-Topic $destConnectionString $useCaseRouteMetadata.DestTopic -Properties @{
		'EnableBatchedOperations' = $true
		'SupportOrdering' = $true
		'RequiresDuplicateDetection' = $true
	}
	Create-Subscription $destConnectionString $useCaseRouteMetadata.DestTopic $useCaseRouteMetadata.DestSubscription -Properties @{
		'EnableBatchedOperations' = $true
		'MaxDeliveryCount' = 0x7fffffff
	}
}

Task Deploy-ConvertUseCasesService -Depends Create-Topics -Precondition { $Metadata['ConvertUseCasesService'] -and $Metadata['UseCaseRoute'] } {
	
	Load-WinServiceModule 'ConvertUseCasesService'
	Take-WinServiceOffline 'ConvertUseCasesService'

	if ($Metadata.UseCaseRoute.RouteName -eq 'ERM'){
		Remove-WinService 'ConvertUseCasesService'
	} else {
		Deploy-WinService 'ConvertUseCasesService'
	}
}