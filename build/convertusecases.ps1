Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

Task Build-ConvertUseCasesService {

	if ($Metadata['ConvertUseCasesService']){
		Build-ConvertUseCasesService 'ConvertUseCasesService'
	}

	if ($Metadata['ConvertUseCasesService-Production']){
		Build-ConvertUseCasesService 'ConvertUseCasesService-Production'
	}
}

function Build-ConvertUseCasesService ($entryPointName) {

	$configFileName = Get-ConvertUseCasesServiceConfig
	$transformedConfig = Get-TransformedConfig $configFileName $entryPointName

	$serviceBusMetadata = $Metadata[$entryPointName].ServiceBus
	if ($serviceBusMetadata.UseCaseRoute -match 'ERMProduction'){

		# дополнительная трансформация connection string для production source
		$connectionStringName = 'Source'

		$xmlNode = $transformedConfig.SelectSingleNode("configuration/connectionStrings/add[@name = '$connectionStringName']")
		if ($xmlNode -eq $null){
			throw "Could not find connection string '$ConnectionStringName'"
		}

		$productionConfig = Get-TransformedConfig $configFileName 'ConvertUseCasesService-Production'
		$productionConnectionString = Get-ConnectionString $productionConfig $connectionStringName

		$xmlNode.connectionString = $productionConnectionString
	}

	$tempDir = Copy-ConvertUseCasesServiceToTempDir
	$transformedconfigFileName = Join-Path $tempDir (Split-Path $configFileName -Leaf)
	$transformedConfig.Save($transformedconfigFileName)
	Publish-Artifacts $tempDir $entryPointName
}


function Get-ConvertUseCasesServiceConfig {
	$packageInfo = Get-PackageInfo '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases'
	$toolsDir = Join-Path $packageInfo.VersionedDir 'tools'

	$configFileName = Join-Path $toolsDir '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases.exe.config'
	return $configFileName
}

function Copy-ConvertUseCasesServiceToTempDir {
	$packageInfo = Get-PackageInfo '2GIS.NuClear.AdvancedSearch.Tools.ConvertTrackedUseCases'
	$toolsDir = Join-Path $packageInfo.VersionedDir 'tools'

	$tempDir = Join-Path $Metadata.Common.Dir.Temp 'ConvertUseCasesService'
	if (!(Test-Path $tempDir)){
		Copy-Item $toolsDir $tempDir -Force -Recurse
	}
	
	return $tempDir
}

function QueueDeploy-ConvertUseCasesService {
	if ($Metadata['ConvertUseCasesService']){

		$useCaseRoute = $Metadata['ConvertUseCasesService'].ServiceBus.UseCaseRoute
		if ($useCaseRoute -eq 'ERMProduction'){
			QueueDeploy-WinService 'ConvertUseCasesService'
		} else {
			# just remove service without deploy
			Add-DeployQueue "Delete ConvertUseCasesService" {
				param($localPSScriptRoot, $localMetadata, $entryPointMetadataKey)

				Import-Module "$localPSScriptRoot\metadata.psm1" -DisableNameChecking
				Add-Metadata $localMetadata

				Import-Module "$localPSScriptRoot\deploy.winservice.psm1" -DisableNameChecking

				Load-WinServiceModule $entryPointMetadataKey
				Take-WinServiceOffline $entryPointMetadataKey
				Remove-WinService $entryPointMetadataKey
		
			} -ArgumentList @("$BuildToolsRoot\modules", $Metadata, 'ConvertUseCasesService')			
		}
	}
}